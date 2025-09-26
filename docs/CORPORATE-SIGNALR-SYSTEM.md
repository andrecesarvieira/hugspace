# 📡 Sistema SignalR - Comunicação Corporativa em Tempo Real

## 📋 Visão Geral

O **Sistema SignalR** do SynQcore implementa comunicação corporativa em tempo real, desenvolvido na **Fase 4.1** com dois Hubs especializados para diferentes tipos de comunicação empresarial.

## 🏗️ Arquitetura SignalR

### **🤝 CorporateCollaborationHub**
```csharp
[Authorize]
public partial class CorporateCollaborationHub : Hub
```

#### **Funcionalidades de Colaboração**

##### **🎯 Canais de Equipe**
```csharp
// Entrar em canal de equipe
await connection.invoke("JoinTeamChannel", "team-dev");

// Enviar mensagem para equipe
await connection.invoke("SendTeamMessage", "team-dev", "Reunião às 14h!");

// Sair do canal
await connection.invoke("LeaveTeamChannel", "team-dev");
```

##### **📂 Canais de Projeto**  
```csharp
// Colaboração em projetos específicos
await connection.invoke("JoinProjectChannel", "projeto-mobile");
await connection.invoke("SendProjectMessage", "projeto-mobile", "Deploy realizado com sucesso!");
await connection.invoke("LeaveProjectChannel", "projeto-mobile");
```

##### **👤 Sistema de Presença**
```csharp
// Atualizar status de presença
await connection.invoke("UpdatePresenceStatus", "Available");  // Available, Busy, Away, InMeeting
```

#### **Eventos Recebidos**
```javascript
// Usuário entrou online
connection.on("UserOnline", (data) => {
    console.log(`${data.Email} ficou online`);
    updateUserPresence(data.UserId, "online");
});

// Mensagem de equipe recebida
connection.on("ReceiveTeamMessage", (data) => {
    addMessageToTeamChat(data.TeamId, data.Message, data.UserEmail);
});

// Mudança de presença
connection.on("UserPresenceChanged", (data) => {
    updateUserStatus(data.UserId, data.Status);
});
```

---

### **🏢 ExecutiveCommunicationHub** 
```csharp
[Authorize]
public partial class ExecutiveCommunicationHub : Hub
```

#### **Comunicações Executivas**

##### **📢 Anúncios Corporativos**
```csharp
[Authorize(Roles = "Manager,HR,Admin")]
public async Task SendCompanyAnnouncement(string title, string message, string priority = "Normal")
```

**Exemplo de uso:**
```javascript
// Enviar anúncio para toda empresa (apenas Manager/HR/Admin)
await executiveConnection.invoke("SendCompanyAnnouncement", 
    "Nova Política de Home Office", 
    "A partir de segunda-feira, nova política entra em vigor...", 
    "High"
);
```

##### **🔒 Comunicações Executivas Confidenciais**
```csharp
[Authorize(Roles = "Manager,HR,Admin")]
public async Task SendExecutiveCommunication(string title, string message, string confidentialityLevel = "Internal")
```

##### **📋 Atualizações de Políticas**
```csharp
[Authorize(Roles = "HR,Admin")]
public async Task SendPolicyUpdate(string policyTitle, string changeDescription, string effectiveDate, bool requiresAcknowledgment = false)
```

##### **🏢 Comunicações Departamentais**
```csharp
// Entrar em comunicações do departamento
await connection.invoke("JoinDepartmentCommunications", "dept-desenvolvimento");

// Enviar comunicado departamental (Manager+)
await connection.invoke("SendDepartmentCommunication", 
    "dept-desenvolvimento", 
    "Sprint Review", 
    "Review agendada para sexta às 16h"
);
```

#### **Eventos Executivos Recebidos**
```javascript
// Anúncio corporativo recebido
connection.on("ReceiveCompanyAnnouncement", (data) => {
    showNotification(data.Title, data.Message, data.Priority);
});

// Atualização de política
connection.on("ReceivePolicyUpdate", (data) => {
    if (data.RequiresAcknowledgment) {
        showPolicyAcknowledgmentDialog(data);
    }
});

// Comunicação departamental
connection.on("ReceiveDepartmentCommunication", (data) => {
    addDepartmentMessage(data.DepartmentId, data.Title, data.Message);
});
```

## 🔐 Autenticação e Autorização

### **JWT Authentication**
```javascript
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/hubs/corporate-collaboration", {
        accessTokenFactory: () => localStorage.getItem("jwt_token")
    })
    .build();
```

### **Grupos Automáticos**
- **Role Groups**: `Role_Admin`, `Role_Manager`, `Role_HR`, `Role_Employee`
- **Team Groups**: `Team_{teamId}` para colaboração de equipe  
- **Project Groups**: `Project_{projectId}` para projetos
- **Department Groups**: `Department_{departmentId}` para comunicações departamentais
- **Company Groups**: `CompanyAnnouncements` (todos), `ExecutiveBroadcast` (Manager+)

### **Permissões por Funcionalidade**

| Funcionalidade | Employee | Manager | HR | Admin |
|----------------|----------|---------|-------|-------|
| **Join Team/Project Channels** | ✅ | ✅ | ✅ | ✅ |
| **Send Team/Project Messages** | ✅ | ✅ | ✅ | ✅ |  
| **Update Presence Status** | ✅ | ✅ | ✅ | ✅ |
| **Send Company Announcement** | ❌ | ✅ | ✅ | ✅ |
| **Send Executive Communication** | ❌ | ✅ | ✅ | ✅ |
| **Send Policy Update** | ❌ | ❌ | ✅ | ✅ |
| **Send Department Communication** | ❌ | ✅ | ✅ | ✅ |

## ⚡ Performance e Logging

### **LoggerMessage Delegates**
```csharp
// Event IDs 4001-4009 (CorporateCollaborationHub)
[LoggerMessage(EventId = 4001, Level = LogLevel.Information,
    Message = "Usuário conectado: {Email} ({Role}) | ConnectionId: {ConnectionId}")]
private static partial void LogUserConnected(ILogger logger, string email, string role, string connectionId);

// Event IDs 4101-4109 (ExecutiveCommunicationHub) 
[LoggerMessage(EventId = 4101, Level = LogLevel.Information,
    Message = "Executivo conectado ao hub: {Email} ({Role}) | ConnectionId: {ConnectionId}")]
private static partial void LogExecutiveConnected(ILogger logger, string email, string role, string connectionId);
```

### **Event IDs Organizados**
- **4001-4009**: CorporateCollaborationHub (conectar, canais, mensagens, presença)
- **4101-4109**: ExecutiveCommunicationHub (executivos, anúncios, políticas, departamentos)

## 🌐 REST API de Documentação

### **SignalRDocumentationController**

#### **📝 Endpoints Disponíveis**
```http
GET /api/SignalRDocumentation/hubs                    # Informações dos Hubs
GET /api/SignalRDocumentation/examples/javascript     # Exemplos JavaScript
GET /api/SignalRDocumentation/status                  # Status conexões (Admin)
GET /api/SignalRDocumentation/implementation-guide    # Guia implementação
```

#### **Exemplo de Resposta - Hubs Info**
```json
{
  "CorporateCollaborationHub": {
    "Name": "CorporateCollaborationHub",
    "Endpoint": "/hubs/corporate-collaboration", 
    "Description": "Hub principal para colaboração corporativa em tempo real",
    "Authentication": "JWT Bearer Token via query string (?access_token=TOKEN)",
    "Methods": "JoinTeamChannel, SendTeamMessage, UpdatePresenceStatus...",
    "Events": "UserOnline, ReceiveTeamMessage, UserPresenceChanged..."
  },
  "ExecutiveCommunicationHub": {
    "Name": "ExecutiveCommunicationHub",
    "Endpoint": "/hubs/executive-communication",
    "Description": "Hub especializado para comunicações executivas",
    "Methods": "SendCompanyAnnouncement, SendPolicyUpdate...",
    "Events": "ReceiveCompanyAnnouncement, ReceivePolicyUpdate..."
  }
}
```

## 🔧 Configuração Técnica

### **Program.cs - Setup**
```csharp
// Configurar SignalR
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = builder.Environment.IsDevelopment();
    options.KeepAliveInterval = TimeSpan.FromSeconds(30);
    options.ClientTimeoutInterval = TimeSpan.FromSeconds(60);
});

// Mapear Hubs
app.MapHub<CorporateCollaborationHub>("/hubs/corporate-collaboration");
app.MapHub<ExecutiveCommunicationHub>("/hubs/executive-communication");
```

### **JWT Authentication para SignalR**
```csharp
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                // Suporte para JWT via query string (SignalR WebSocket)
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;
                
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });
```

## 📱 Implementação Cliente JavaScript

### **Conectar aos Hubs**
```javascript
// Conectar ao Hub de Colaboração
const collaborationConnection = new signalR.HubConnectionBuilder()
    .withUrl("/hubs/corporate-collaboration", {
        accessTokenFactory: () => localStorage.getItem("jwt_token")
    })
    .configureLogging(signalR.LogLevel.Information)
    .build();

// Conectar ao Hub Executivo  
const executiveConnection = new signalR.HubConnectionBuilder()
    .withUrl("/hubs/executive-communication", {
        accessTokenFactory: () => localStorage.getItem("jwt_token")
    })
    .build();
```

### **Tratamento de Reconexão**
```javascript
// Configurar reconexão automática
collaborationConnection.onclose(async (error) => {
    console.error("Conexão perdida:", error);
    await reconnectWithBackoff();
});

// Função de reconexão com backoff exponencial
async function reconnectWithBackoff() {
    let delay = 1000; // Começar com 1s
    
    while (collaborationConnection.state !== signalR.HubConnectionState.Connected) {
        try {
            await collaborationConnection.start();
            console.log("Reconectado com sucesso!");
            break;
        } catch (error) {
            console.warn(`Tentativa de reconexão falhou. Tentando novamente em ${delay}ms`);
            await new Promise(resolve => setTimeout(resolve, delay));
            delay = Math.min(delay * 2, 30000); // Max 30s
        }
    }
}
```

### **Configuração Completa de Events**
```javascript
// === COLLABORATION HUB EVENTS ===
collaborationConnection.on("UserOnline", (data) => {
    updateUserPresence(data.UserId, "online", data.Role);
    showToast(`${data.Email} ficou online`, "success");
});

collaborationConnection.on("ReceiveTeamMessage", (data) => {
    addMessageToTeamChat(data.TeamId, {
        message: data.Message,
        sender: data.UserEmail,
        timestamp: data.Timestamp
    });
});

collaborationConnection.on("UserPresenceChanged", (data) => {
    updateUserStatusIndicator(data.UserId, data.Status);
});

// === EXECUTIVE HUB EVENTS ===
executiveConnection.on("ReceiveCompanyAnnouncement", (data) => {
    showCompanyAnnouncement({
        title: data.Title,
        message: data.Message,
        priority: data.Priority,
        sender: data.SentBy
    });
});

executiveConnection.on("ReceivePolicyUpdate", (data) => {
    if (data.RequiresAcknowledgment) {
        showPolicyAcknowledgmentModal(data);
    } else {
        showPolicyNotification(data);
    }
});
```

## 🚀 Uso Avançado

### **Dashboard de Status em Tempo Real**
```javascript
// Conectar e monitorar presença de equipe
await collaborationConnection.start();
await collaborationConnection.invoke("JoinTeamChannel", currentUser.teamId);

// Atualizar presença baseada em atividade do usuário
document.addEventListener("visibilitychange", async () => {
    const status = document.hidden ? "Away" : "Available";
    await collaborationConnection.invoke("UpdatePresenceStatus", status);
});

// Detectar inatividade
let inactivityTimer;
document.addEventListener("mousemove", resetInactivityTimer);
document.addEventListener("keypress", resetInactivityTimer);

function resetInactivityTimer() {
    clearTimeout(inactivityTimer);
    inactivityTimer = setTimeout(async () => {
        await collaborationConnection.invoke("UpdatePresenceStatus", "Away");
    }, 5 * 60 * 1000); // 5 minutos
}
```

### **Sistema de Notificações Corporativas**
```javascript
// Configurar notificações do browser
if ("Notification" in window && Notification.permission === "granted") {
    executiveConnection.on("ReceiveCompanyAnnouncement", (data) => {
        new Notification(data.Title, {
            body: data.Message,
            icon: "/icons/company-announcement.png",
            badge: "/icons/synqcore-badge.png",
            tag: "company-announcement"
        });
    });
}

// Sistema de priorização de notificações
const priorityConfig = {
    "Emergency": { sound: "urgent.mp3", persistent: true },
    "High": { sound: "high.mp3", persistent: false },
    "Normal": { sound: "normal.mp3", persistent: false }
};
```

## 💡 Casos de Uso Corporativo

### **1. Reunião de Equipe em Tempo Real**
```javascript
// Manager inicia reunião
await collaborationConnection.invoke("JoinTeamChannel", "team-dev");
await collaborationConnection.invoke("SendTeamMessage", "team-dev", "🎯 Daily Standup iniciado!");

// Membros entram automaticamente
team.members.forEach(async (member) => {
    await collaborationConnection.invoke("UpdatePresenceStatus", "InMeeting");
});
```

### **2. Comunicado Executivo Urgente**
```javascript
// HR envia política urgente
await executiveConnection.invoke("SendPolicyUpdate",
    "Política de Segurança Atualizada",
    "Nova política de senhas obrigatória a partir de amanhã.",
    new Date().toISOString(),
    true // Requer confirmação
);
```

### **3. Comunicação Departamental Direcionada**  
```javascript
// Manager comunica apenas ao seu departamento
await executiveConnection.invoke("JoinDepartmentCommunications", "dept-ti");
await executiveConnection.invoke("SendDepartmentCommunication",
    "dept-ti",
    "Manutenção Programada",
    "Servidores em manutenção sábado das 6h às 10h."
);
```

## ✅ Status de Implementação

- ✅ **CorporateCollaborationHub** (100% - 9 métodos + 5 eventos)
- ✅ **ExecutiveCommunicationHub** (100% - 6 métodos + 4 eventos)  
- ✅ **JWT Authentication** (100% - Query string support)
- ✅ **Role-Based Authorization** (100% - 4 levels)
- ✅ **LoggerMessage Performance** (100% - 18 delegates)
- ✅ **Documentation Controller** (100% - 4 endpoints)
- ✅ **Program.cs Configuration** (100% - SignalR setup)
- ✅ **JavaScript Examples** (100% - Complete implementation guide)
- ✅ **Error Handling & Reconnection** (100% - Production ready)

---

**Sistema implementado em:** 26 de Setembro de 2025  
**Versão:** 4.1.0  
**Status:** ✅ Produção Ready  
**Endpoints:** `/hubs/corporate-collaboration` + `/hubs/executive-communication`  
**Autor:** André César Vieira (@andrecesarvieira)