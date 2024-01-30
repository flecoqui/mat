#!/bin/bash
##########################################################################################################################################################################################
#- Purpose: Build the self contained releases
#- Parameters are:
#-  None
#


set -u
parent_path=$(
    cd "$(dirname "${BASH_SOURCE[0]}")/../../"
    pwd -P
)

#######################################################
#- build and archive
# $1 = runtime name win-x64, linux-x64, linux-musl-x64, rhel-x64, osx-x64

#######################################################
build () {
  RUN_TIME=$1

  dotnet publish "${SCRIPTS_DIRECTORY}/../src/MediaArchiveTool/MediaArchiveTool.csproj" --self-contained -c Release -r ${RUN_TIME}
  pushd "${SCRIPTS_DIRECTORY}/../src/MediaArchiveTool/bin/Release/net6.0/${RUN_TIME}/publish" > /dev/null
  tar -czf "../../../../../../../${RELEASES_FOLDER}/mat-${RUN_TIME}.tar.gz" .
  popd > /dev/null
}

SCRIPTS_DIRECTORY=$(dirname "$0")
RELEASES_FOLDER="releases"

if [ ! -d "${SCRIPTS_DIRECTORY}/../${RELEASES_FOLDER}" ]; then
    mkdir "${SCRIPTS_DIRECTORY}/../${RELEASES_FOLDER}"
fi

build win-x64
build linux-x64
build linux-musl-x64
build rhel-x64
build osx-x64