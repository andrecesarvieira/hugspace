# Script wrapper para Windows PowerShell - SynQcore
# Chama o script Python com os argumentos fornecidos

param(
    [Parameter(ValueFromRemainingArguments=$true)]
    [string[]]$Arguments
)

python "scripts\synqcore.py" @Arguments