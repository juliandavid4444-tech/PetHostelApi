# Guía de Despliegue de PetHostelApi en Azure

## Pasos para Publicar en Azure App Service

### 1. Preparación Completada ✅
- ✅ CORS configurado para aplicaciones móviles
- ✅ Archivos de configuración de producción creados
- ✅ Extensiones de Azure instaladas en VS Code
- 🔄 Azure CLI instalándose...

### 2. Opciones de Despliegue

#### Opción A: Despliegue desde VS Code (Recomendado para principiantes)

1. **Hacer login en Azure desde VS Code:**
   - Presiona `Cmd+Shift+P` (macOS) 
   - Busca "Azure: Sign In"
   - Sigue las instrucciones para loguearte

2. **Crear App Service:**
   - Ve a la extensión de Azure en el sidebar izquierdo
   - Click en "Create new Azure App Service"
   - Sigue el wizard:
     - Nombre: `pethostel-api-[tu-nombre]`
     - OS: Linux
     - Runtime: .NET 8 (LTS)
     - Región: East US o la más cercana a ti

3. **Desplegar:**
   - Right-click en tu proyecto en el explorador
   - Selecciona "Deploy to Web App"
   - Selecciona tu App Service creado
   - ¡Listo!

#### Opción B: Usando Azure CLI (Más control)

Cuando termine de instalar Azure CLI, ejecutar estos comandos:

```bash
# 1. Login en Azure
az login

# 2. Crear Resource Group
az group create --name pethostel-rg --location "East US"

# 3. Crear App Service Plan
az appservice plan create --name pethostel-plan --resource-group pethostel-rg --sku B1 --is-linux

# 4. Crear Web App
az webapp create --resource-group pethostel-rg --plan pethostel-plan --name pethostel-api-[tu-nombre] --runtime "DOTNETCORE:8.0"

# 5. Desplegar código
az webapp up --resource-group pethostel-rg --name pethostel-api-[tu-nombre] --runtime "DOTNETCORE:8.0"
```

### 3. Configurar Base de Datos

Tu API ya está configurada para conectarse a tu base de datos de Azure SQL:
- Server: `pethostel.database.windows.net`
- Database: `PetHostelDB`
- User: `juliandavid333`

**IMPORTANTE:** En producción, considera usar Azure Key Vault para las credenciales.

### 4. URL Final

Una vez desplegado, tu API estará disponible en:
`https://pethostel-api-[tu-nombre].azurewebsites.net`

### 5. Endpoints para tu App Móvil

- **Login:** `POST https://pethostel-api-[tu-nombre].azurewebsites.net/Auth/login`
- **Swagger UI:** `https://pethostel-api-[tu-nombre].azurewebsites.net/swagger`

### 6. Configuración Adicional Recomendada

#### Configurar HTTPS Only:
```bash
az webapp update --resource-group pethostel-rg --name pethostel-api-[tu-nombre] --https-only true
```

#### Configurar Variables de Entorno (si es necesario):
```bash
az webapp config appsettings set --resource-group pethostel-rg --name pethostel-api-[tu-nombre] --settings ASPNETCORE_ENVIRONMENT=Production
```

### 7. Testing

Una vez desplegado, puedes probar tu API:

```bash
# Test de login
curl -X POST "https://pethostel-api-[tu-nombre].azurewebsites.net/Auth/login" \
  -H "Content-Type: application/json" \
  -d '{"user": "tu_usuario", "password": "tu_password"}'
```

### 8. Monitoreo

- Ve al Azure Portal: https://portal.azure.com
- Busca tu App Service
- Ve a "Application Insights" para monitoreo
- Revisa "Log stream" para logs en tiempo real

---

## ¿Cuál método prefieres usar?

1. **VS Code (Fácil):** Solo unos clicks, perfecto para empezar
2. **Azure CLI (Control):** Más comandos pero más control del proceso

¡Una vez desplegado podrás conectar tu app móvil directamente a la API en Azure!