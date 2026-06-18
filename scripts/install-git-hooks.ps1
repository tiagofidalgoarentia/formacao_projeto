<#
.SYNOPSIS
Configura os hooks Git versionados deste repositorio.

.DESCRIPTION
Define core.hooksPath para a pasta .githooks, permitindo que o pre-commit
versionado seja executado antes de cada commit local.
#>
[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'

$repositoryRoot = Resolve-Path (Join-Path $PSScriptRoot '..')
$hooksPath = Join-Path $repositoryRoot '.githooks'

if (-not (Test-Path $hooksPath -PathType Container)) {
    throw "Pasta de hooks nao encontrada: $hooksPath"
}

git -C $repositoryRoot config core.hooksPath .githooks

Write-Host 'Git hooks configurados em .githooks.'
Write-Host 'O pre-commit vai executar dotnet format --verify-no-changes e dotnet build.'
