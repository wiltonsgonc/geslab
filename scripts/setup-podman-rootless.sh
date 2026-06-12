#!/bin/bash
set -euo pipefail

echo "========================================"
echo "  Configurar Podman Rootless"
echo "  (Alma Linux / RHEL / Debian / Ubuntu / WSL2)"
echo "========================================"
echo ""

if [ "$(id -u)" -eq 0 ]; then
  echo "ERRO: Nao execute como root. Execute como o usuario do Podman."
  exit 1
fi

if ! command -v podman &>/dev/null; then
  echo "ERRO: Podman nao instalado."
  echo "  Alma/RHEL: sudo dnf install -y podman podman-compose"
  echo "  Debian/Ubuntu: sudo apt install -y podman podman-compose"
  exit 1
fi

PODMAN_VER=$(podman version --format '{{.Server.Version}}' 2>/dev/null | cut -d. -f1 || echo "0")
echo "  Podman major version: $PODMAN_VER"

UID_NUM=$(id -u)
RUNTIME_DIR="/run/user/${UID_NUM}"
CUSER=$(whoami)
SHELL_RC="${HOME}/.bashrc"
[ -f "${HOME}/.bash_profile" ] && SHELL_RC="${HOME}/.bash_profile"

# ==============================================
# [1/9] containers.conf
# ==============================================
echo ""
echo "[1/9] Criando containers.conf..."
mkdir -p ~/.config/containers

# Podman 5+ usa pasta + aardvark-dns, ambos precisam de systemd --user.
# dns_bind_port = 0  -> desliga aardvark-dns (causa do erro fatal)
# default_rootless_network_cmd = "slirp4netns" -> evita pasta (causa do erro secundario)
# cgroup_manager = "cgroupfs" -> evita cgroups via systemd
cat > ~/.config/containers/containers.conf << 'EOF'
[engine]
cgroup_manager = "cgroupfs"
events_logger = "file"

[network]
dns_bind_port = 0
default_rootless_network_cmd = "slirp4netns"
EOF
echo "  OK: ~/.config/containers/containers.conf criado"

# ==============================================
# [2/9] crun.conf
# ==============================================
echo ""
echo "[2/9] Criando crun.conf..."
mkdir -p ~/.config/crun
cat > ~/.config/crun/crun.conf << 'EOF'
cgroup:
  manager: "cgroupfs"
EOF
echo "  OK: ~/.config/crun/crun.conf criado"

# ==============================================
# [3/9] subuid / subgid
# ==============================================
echo ""
echo "[3/9] Verificando subuid/subgid..."
if ! grep -q "^${CUSER}:" /etc/subuid 2>/dev/null; then
  echo "  Configurando subuid/subgid para $CUSER..."
  sudo usermod --add-subuids 100000-165535 --add-subgids 100000-165535 "$CUSER"
  echo "  OK"
else
  echo "  OK: subuid/subgid ja configurados para $CUSER"
fi

# ==============================================
# [4/9] Lingering
# ==============================================
echo ""
echo "[4/9] Habilitando lingering..."
sudo loginctl enable-linger "$CUSER" 2>/dev/null || true
echo "  OK: loginctl enable-linger $CUSER"

# ==============================================
# [5/9] XDG_RUNTIME_DIR
# ==============================================
echo ""
echo "[5/9] Configurando XDG_RUNTIME_DIR (${RUNTIME_DIR})..."
sudo mkdir -p "${RUNTIME_DIR}"
sudo chown "${CUSER}:${CUSER}" "${RUNTIME_DIR}"
chmod 700 "${RUNTIME_DIR}"
echo "  OK: ${RUNTIME_DIR} criado"

export XDG_RUNTIME_DIR="${RUNTIME_DIR}"
export DBUS_SESSION_BUS_ADDRESS="unix:path=${RUNTIME_DIR}/bus"
echo "  OK: variaveis exportadas"

# ==============================================
# [6/9] Persiste variaveis no shell
# ==============================================
echo ""
echo "[6/9] Persistindo variaveis no ${SHELL_RC}..."

EXPORT_BLOCK="
# --- Podman rootless (adicionado por setup-podman-rootless.sh) ---
export XDG_RUNTIME_DIR=/run/user/\$(id -u)
export DBUS_SESSION_BUS_ADDRESS=unix:path=/run/user/\$(id -u)/bus
# ----------------------------------------------------------------"

if ! grep -q "Podman rootless" "${SHELL_RC}" 2>/dev/null; then
  echo "${EXPORT_BLOCK}" >> "${SHELL_RC}"
  echo "  OK: variaveis adicionadas em ${SHELL_RC}"
else
  echo "  OK: variaveis ja presentes em ${SHELL_RC}"
fi

# ==============================================
# [7/9] Remove composicao existente e redes
# OBS: a rede do compose foi criada na primeira execucao
# com aardvark-dns habilitado. Mesmo com a config nova,
# a rede antiga persiste e tenta usar aardvark.
# ==============================================
echo ""
echo "[7/9] Removendo redes existentes do Podman..."
podman network prune -f 2>/dev/null || true
# Remove redes do projeto lab especificamente
podman network ls --format '{{.Name}}' 2>/dev/null | grep -E '^lab_' | while read -r net; do
  echo "  Removendo rede: $net"
  podman network rm "$net" 2>/dev/null || true
done
echo "  OK: redes removidas"

# ==============================================
# [8/9] Migra storage do Podman
# ==============================================
echo ""
echo "[8/9] Migrando Podman (storage e rede)..."
podman system migrate 2>/dev/null || true
echo "  OK"

# ==============================================
# [9/9] Verifica podman-compose
# ==============================================
echo ""
echo "[9/9] Verificando podman-compose..."
if command -v podman-compose &>/dev/null; then
  CURRENT_VER=$(podman-compose version 2>/dev/null | grep -oP '\d+\.\d+' | head -1 || echo "0.0")
  echo "  Versao: $CURRENT_VER"
  MAJOR=$(echo "$CURRENT_VER" | cut -d. -f1)
  MINOR=$(echo "$CURRENT_VER" | cut -d. -f2)
  if [[ "$MAJOR" -lt 1 ]] || { [[ "$MAJOR" -eq 1 ]] && [[ "$MINOR" -lt 2 ]]; }; then
    echo "  AVISO: versao $CURRENT_VER nao suporta 'condition: service_healthy'."
    echo "  Atualize: pip3 install --upgrade podman-compose"
  fi
else
  if command -v pip3 &>/dev/null; then
    echo "  Instalando podman-compose via pip3..."
    pip3 install --user podman-compose
    export PATH="${HOME}/.local/bin:${PATH}"
    echo "  OK"
  else
    echo "  AVISO: pip3 nao encontrado."
    echo "  sudo dnf install -y python3-pip && pip3 install --user podman-compose"
  fi
fi

# ==============================================
# Verificacao final
# ==============================================
echo ""
echo "========================================"
echo "  Verificando configuracao..."
echo "========================================"

echo ""
echo "--- containers.conf ---"
cat ~/.config/containers/containers.conf 2>/dev/null || echo "  (nao encontrado)"

echo ""
echo "--- crun.conf ---"
cat ~/.config/crun/crun.conf 2>/dev/null || echo "  (nao encontrado)"

echo ""
CGROUP_MGR=$(podman info --format '{{.Host.CgroupManager}}' 2>/dev/null || echo "desconhecido")
OCI_RT=$(podman info --format '{{.Host.OCIRuntime.Name}}' 2>/dev/null || echo "desconhecido")
echo "Cgroup Manager : $CGROUP_MGR"
echo "OCI Runtime    : $OCI_RT"

echo ""
echo "========================================"
echo "  Testando Podman..."
echo "========================================"

if podman run --rm docker.io/library/alpine echo "Podman rootless OK!" 2>/dev/null; then
  echo ""
  echo "========================================"
  echo "  Configuracao concluida com sucesso!"
  echo "========================================"
  echo ""
  echo "IMPORTANTE: Abra UMA NOVA SESSAO SSH e execute:"
  echo ""
  echo "  source ~/.bashrc"
  echo "  podman-compose up -d"
else
  echo ""
  echo "========================================"
  echo "  AVISO: Teste falhou."
  echo "========================================"
  echo ""
  echo "Tente manualmente:"
  echo "  podman run --rm alpine echo ok"
  echo ""
  echo "Se ainda falhar, use o fallback com host networking:"
  echo "  podman-compose -f docker-compose.yml -f docker-compose.podman.yml --env-file .env.podman up -d"
fi
