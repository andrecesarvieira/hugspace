// === FUNÇÕES JAVASCRIPT PARA O FEED === //

// Função para mostrar notificações
window.showNotification = (message, type = 'info') => {
    // Criar o elemento de notificação
    const notification = document.createElement('div');
    notification.className = `notification notification-${type}`;
    notification.innerHTML = `
        <div class="notification-content">
            <i class="fas ${getNotificationIcon(type)}"></i>
            <span>${message}</span>
        </div>
        <button class="notification-close" onclick="this.parentElement.remove()">
            <i class="fas fa-times"></i>
        </button>
    `;
    
    // Adicionar estilos
    notification.style.cssText = `
        position: fixed;
        top: 20px;
        right: 20px;
        z-index: 10000;
        background: ${getNotificationColor(type)};
        color: white;
        padding: 16px 20px;
        border-radius: 8px;
        box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
        display: flex;
        align-items: center;
        justify-content: space-between;
        gap: 12px;
        max-width: 400px;
        animation: slideInRight 0.3s ease-out;
    `;
    
    // Adicionar ao body
    document.body.appendChild(notification);
    
    // Remover automaticamente após 5 segundos
    setTimeout(() => {
        if (notification.parentElement) {
            notification.style.animation = 'slideOutRight 0.3s ease-in';
            setTimeout(() => notification.remove(), 300);
        }
    }, 5000);
};

// Função para copiar texto para a área de transferência
window.copyToClipboard = async (text) => {
    try {
        await navigator.clipboard.writeText(text);
        return true;
    } catch (err) {
        // Fallback para navegadores mais antigos
        const textArea = document.createElement('textarea');
        textArea.value = text;
        document.body.appendChild(textArea);
        textArea.focus();
        textArea.select();
        try {
            document.execCommand('copy');
            document.body.removeChild(textArea);
            return true;
        } catch (err) {
            document.body.removeChild(textArea);
            return false;
        }
    }
};

// Função para mostrar opções de compartilhamento
window.showShareOptions = (postId) => {
    const currentUrl = `${window.location.origin}/post/${postId}`;
    const postElement = document.querySelector(`[data-post-id="${postId}"]`);
    const postContent = postElement?.querySelector('.post-text')?.innerText || '';
    const shareText = `Confira este post: ${postContent.substring(0, 100)}${postContent.length > 100 ? '...' : ''}`;
    
    // Verificar se a API de compartilhamento nativo está disponível
    if (navigator.share) {
        navigator.share({
            title: 'SynQcore - Post',
            text: shareText,
            url: currentUrl
        }).catch(err => console.log('Erro ao compartilhar:', err));
    } else {
        // Fallback - copiar link
        copyToClipboard(currentUrl);
        showNotification('Link copiado para a área de transferência!', 'success');
    }
};

// Função para mostrar efeito de like
window.showLikeEffect = (postId) => {
    const postElement = document.querySelector(`[data-post-id="${postId}"]`);
    if (!postElement) return;
    
    const likeButton = postElement.querySelector('.like-button');
    if (!likeButton) return;
    
    // Criar elementos de animação
    for (let i = 0; i < 5; i++) {
        setTimeout(() => {
            const heart = document.createElement('div');
            heart.innerHTML = '❤️';
            heart.style.cssText = `
                position: absolute;
                pointer-events: none;
                font-size: 1.5rem;
                z-index: 1000;
                animation: heartFloat 1s ease-out forwards;
                left: ${Math.random() * 40 - 20}px;
                top: -10px;
            `;
            
            likeButton.style.position = 'relative';
            likeButton.appendChild(heart);
            
            setTimeout(() => heart.remove(), 1000);
        }, i * 100);
    }
};

// Função para focar um elemento
window.focusElement = (selector) => {
    const element = document.querySelector(selector);
    if (element) {
        element.focus();
    }
};

// Funções auxiliares
function getNotificationIcon(type) {
    switch (type) {
        case 'success': return 'fa-check-circle';
        case 'error': return 'fa-exclamation-circle';
        case 'warning': return 'fa-exclamation-triangle';
        case 'info': 
        default: return 'fa-info-circle';
    }
}

function getNotificationColor(type) {
    switch (type) {
        case 'success': return '#10b981';
        case 'error': return '#ef4444';
        case 'warning': return '#f59e0b';
        case 'info': 
        default: return '#3b82f6';
    }
}

// Adicionar estilos CSS para as animações
const style = document.createElement('style');
style.textContent = `
    @keyframes slideInRight {
        from {
            transform: translateX(100%);
            opacity: 0;
        }
        to {
            transform: translateX(0);
            opacity: 1;
        }
    }
    
    @keyframes slideOutRight {
        from {
            transform: translateX(0);
            opacity: 1;
        }
        to {
            transform: translateX(100%);
            opacity: 0;
        }
    }
    
    @keyframes heartFloat {
        0% {
            transform: translateY(0) scale(0.8);
            opacity: 1;
        }
        50% {
            transform: translateY(-30px) scale(1.2);
            opacity: 0.8;
        }
        100% {
            transform: translateY(-60px) scale(0.6);
            opacity: 0;
        }
    }
    
    .notification-content {
        display: flex;
        align-items: center;
        gap: 8px;
    }
    
    .notification-close {
        background: none;
        border: none;
        color: inherit;
        cursor: pointer;
        opacity: 0.7;
        transition: opacity 0.2s;
    }
    
    .notification-close:hover {
        opacity: 1;
    }
`;
document.head.appendChild(style);

// Função para fechar menus ao clicar fora
document.addEventListener('click', (e) => {
    // Fechar menus de post se clicou fora
    if (!e.target.closest('.post-menu')) {
        document.querySelectorAll('.post-menu-dropdown').forEach(menu => {
            menu.style.display = 'none';
        });
    }
    
    // Fechar menus de reação se clicou fora
    if (!e.target.closest('.action-button')) {
        document.querySelectorAll('.reactions-menu').forEach(menu => {
            menu.style.opacity = '0';
            menu.style.visibility = 'hidden';
            menu.style.pointerEvents = 'none';
        });
    }
});

// Detectar escape para fechar menus
document.addEventListener('keydown', (e) => {
    if (e.key === 'Escape') {
        // Fechar todos os menus
        document.querySelectorAll('.post-menu-dropdown').forEach(menu => {
            menu.style.display = 'none';
        });
        
        document.querySelectorAll('.reactions-menu').forEach(menu => {
            menu.style.opacity = '0';
            menu.style.visibility = 'hidden';
            menu.style.pointerEvents = 'none';
        });
    }
});

console.log('Feed functions loaded successfully!');