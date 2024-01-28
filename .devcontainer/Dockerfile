ARG VARIANT="6.0-bullseye"
FROM mcr.microsoft.com/vscode/devcontainers/dotnet:${VARIANT}

RUN apt-get update && export DEBIAN_FRONTEND=noninteractive \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/*
    
RUN echo 'source /usr/share/bash-completion/completions/git' >> /home/vscode/.bashrc