#!/bin/bash

# Script para detener todos los procesos de PetHostelApi
# Uso: ./stop-pethostel.sh

echo "🔍 Buscando procesos de PetHostelApi..."

# Buscar procesos que contengan "PetHostel" en el nombre
PROCESSES=$(pgrep -f "PetHostel")

if [ -z "$PROCESSES" ]; then
    echo "✅ No se encontraron procesos de PetHostelApi ejecutándose"
else
    echo "🔄 Deteniendo procesos de PetHostelApi..."
    
    # Detener los procesos encontrados
    for pid in $PROCESSES; do
        echo "  - Deteniendo proceso $pid"
        kill -15 "$pid" 2>/dev/null
        
        # Esperar un poco para el shutdown graceful
        sleep 2
        
        # Si aún está corriendo, forzar el cierre
        if kill -0 "$pid" 2>/dev/null; then
            echo "  - Forzando cierre del proceso $pid"
            kill -9 "$pid" 2>/dev/null
        fi
    done
    
    echo "✅ Procesos detenidos"
fi

# Verificar si hay algo usando los puertos comunes
echo "🔍 Verificando puertos..."
for port in 5017 5420 7103 7420 8055 8056; do
    if lsof -i :$port >/dev/null 2>&1; then
        echo "⚠️  Puerto $port aún está en uso:"
        lsof -i :$port
        echo "🔄 Liberando puerto $port..."
        pkill -f ":$port" 2>/dev/null || true
        sleep 1
    else
        echo "✅ Puerto $port está libre"
    fi
done

echo "🎉 Listo para ejecutar PetHostelApi"