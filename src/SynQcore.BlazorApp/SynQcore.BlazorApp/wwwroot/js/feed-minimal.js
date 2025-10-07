// Toast notifications simples para o Feed Minimalista

window.showToast = function(message, type = 'info', duration = 3000) {
    // Remove toasts existentes se houver muitos
    const existingToasts = document.querySelectorAll('.toast-notification');
    if (existingToasts.length >= 3) {
        existingToasts[0].remove();
    }

    // Cria o toast
    const toast = document.createElement('div');
    toast.className = `toast-notification toast-${type}`;
    toast.innerHTML = `
        <div class="toast-content">
            <i class="fas ${getToastIcon(type)} me-2"></i>
            <span>${message}</span>
            <button class="toast-close" onclick="this.parentElement.parentElement.remove()">
                <i class="fas fa-times"></i>
            </button>
        </div>
    `;

    // Estilos inline para o toast
    toast.style.cssText = `
        position: fixed;
        top: 20px;
        right: 20px;
        z-index: 9999;
        min-width: 300px;
        background: white;
        border-radius: 8px;
        box-shadow: 0 4px 12px rgba(0,0,0,0.15);
        border-left: 4px solid ${getToastColor(type)};
        transform: translateX(100%);
        transition: all 0.3s ease;
        margin-bottom: 10px;
    `;

    // Estilos para o conteúdo
    const style = document.createElement('style');
    style.textContent = `
        .toast-content {
            padding: 12px 16px;
            display: flex;
            align-items: center;
            color: #333;
            font-size: 14px;
        }
        .toast-close {
            background: none;
            border: none;
            margin-left: auto;
            padding: 0 0 0 12px;
            cursor: pointer;
            color: #666;
            opacity: 0.7;
        }
        .toast-close:hover {
            opacity: 1;
        }
        .toast-success .toast-content {
            color: #155724;
        }
        .toast-error .toast-content {
            color: #721c24;
        }
        .toast-warning .toast-content {
            color: #856404;
        }
        .toast-info .toast-content {
            color: #0c5460;
        }
    `;
    
    if (!document.querySelector('#toast-styles')) {
        style.id = 'toast-styles';
        document.head.appendChild(style);
    }

    // Adiciona ao DOM
    document.body.appendChild(toast);

    // Anima entrada
    setTimeout(() => {
        toast.style.transform = 'translateX(0)';
    }, 10);

    // Remove automaticamente
    setTimeout(() => {
        toast.style.transform = 'translateX(100%)';
        setTimeout(() => {
            if (toast.parentElement) {
                toast.remove();
            }
        }, 300);
    }, duration);
};

function getToastIcon(type) {
    switch (type) {
        case 'success': return 'fa-check-circle';
        case 'error': return 'fa-exclamation-circle';
        case 'warning': return 'fa-exclamation-triangle';
        case 'info': return 'fa-info-circle';
        default: return 'fa-info-circle';
    }
}

function getToastColor(type) {
    switch (type) {
        case 'success': return '#28a745';
        case 'error': return '#dc3545';
        case 'warning': return '#ffc107';
        case 'info': return '#17a2b8';
        default: return '#17a2b8';
    }
}

// Função para mostrar notificações de console
window.showNotification = function(message, type = 'info') {
    console.log(`[${type.toUpperCase()}] ${message}`);
    showToast(message, type);
};

// Funções de debug
window.debugInfo = function(message) {
    console.log('[DEBUG]', message);
};

// Função para focar inputs (compatibilidade)
window.focusElement = function(elementId) {
    const element = document.getElementById(elementId);
    if (element) {
        element.focus();
    }
};