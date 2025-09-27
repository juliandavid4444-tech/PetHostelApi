#!/bin/bash

# Script para arrancar PetHostelApi de forma limpia
# Uso: ./start-pethostel.sh

echo "🚀 Iniciando PetHostel API de forma limpia..."

# 1. Detener procesos existentes
echo "🧹 Limpiando procesos existentes..."
./stop-pethostel.sh

# 2. Esperar un momento para asegurar que todo se liberó
echo "⏳ Esperando 2 segundos..."
sleep 2

# 3. Ejecutar la aplicación
echo "▶️  Ejecutando aplicación..."
dotnet run --project /Users/juliandavid/Documents/GitHub/PetHostelApi

echo "✅ PetHostel API iniciado en http://localhost:8055"