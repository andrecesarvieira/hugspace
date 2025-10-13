// === GERENCIADOR DE MENUS DO FEED === //

class FeedMenuManager {
    constructor() {
        this.activeMenus = new Set();
        this.init();
    }

    init() {
        // Escutar cliques no documento para fechar menus
        document.addEventListener('click', this.handleDocumentClick.bind(this));
        
        // Escutar tecla ESC para fechar todos os menus
        document.addEventListener('keydown', this.handleKeyDown.bind(this));
        
        // Gerenciar redimensionamento da janela
        window.addEventListener('resize', this.closeAllMenus.bind(this));
        
        console.log('FeedMenuManager inicializado');
    }

    // === MENU DE TRÊS PONTOS === //
    togglePostMenu(postId, buttonElement) {
        const menuId = `post-menu-${postId}`;
        const existingMenu = document.getElementById(menuId);
        
        // Se já existe, toggle
        if (existingMenu) {
            if (existingMenu.style.display === 'block') {
                this.closePostMenu(postId);
                return;
            }
        }
        
        // Fechar outros menus primeiro
        this.closeAllMenus();
        
        // Criar ou mostrar menu
        this.showPostMenu(postId, buttonElement);
    }

    showPostMenu(postId, buttonElement) {
        const menuId = `post-menu-${postId}`;
        let menu = document.getElementById(menuId);
        
        if (!menu) {
            menu = this.createPostMenu(postId);
        }
        
        // Posicionar menu
        this.positionPostMenu(menu, buttonElement);
        
        // Mostrar menu
        menu.style.display = 'block';
        menu.style.opacity = '1';
        menu.style.visibility = 'visible';
        menu.style.pointerEvents = 'all';
        
        // Adicionar à lista de menus ativos
        this.activeMenus.add(menuId);
        
        // Adicionar backdrop se estiver no mobile
        if (window.innerWidth <= 768) {
            this.createBackdrop(menuId);
        }
    }

    createPostMenu(postId) {
        const menu = document.createElement('div');
        menu.id = `post-menu-${postId}`;
        menu.className = 'post-menu-dropdown';
        menu.style.display = 'none';
        
        const menuItems = [
            { icon: 'fa-bookmark', text: 'Salvar post', action: () => this.savePost(postId) },
            { icon: 'fa-link', text: 'Copiar link', action: () => this.copyPostLink(postId) },
            { icon: 'fa-share-alt', text: 'Compartilhar externamente', action: () => this.sharePostExternal(postId) },
            { icon: 'fa-flag', text: 'Reportar conteúdo', action: () => this.reportPost(postId) },
            { icon: 'fa-eye-slash', text: 'Ocultar post', action: () => this.hidePost(postId) },
            { icon: 'fa-user-times', text: 'Deixar de seguir autor', action: () => this.unfollowAuthor(postId) },
            { icon: 'fa-volume-mute', text: 'Silenciar notificações', action: () => this.muteNotifications(postId) }
        ];

        menuItems.forEach(item => {
            const menuItem = document.createElement('button');
            menuItem.className = 'menu-item';
            menuItem.innerHTML = `
                <i class="fas ${item.icon}"></i>
                <span>${item.text}</span>
            `;
            
            menuItem.addEventListener('click', (e) => {
                e.stopPropagation();
                item.action();
                this.closePostMenu(postId);
            });
            
            menu.appendChild(menuItem);
        });
        
        // Adicionar ao container correto (dentro do post-menu para posicionamento relativo)
        const postMenu = document.querySelector(`[data-post-id="${postId}"] .post-menu`);
        if (postMenu) {
            postMenu.appendChild(menu);
        } else {
            document.body.appendChild(menu);
        }
        
        return menu;
    }

    positionPostMenu(menu, buttonElement) {
        const buttonRect = buttonElement.getBoundingClientRect();
        const menuHeight = 400; // Altura estimada do menu
        const menuWidth = 280;
        
        if (window.innerWidth <= 768) {
            // Mobile: menu na parte inferior
            menu.style.position = 'fixed';
            menu.style.bottom = '0';
            menu.style.left = '0';
            menu.style.right = '0';
            menu.style.top = 'auto';
            menu.style.transform = 'none';
            menu.style.width = '100%';
            menu.style.maxWidth = 'none';
            menu.style.borderRadius = '16px 16px 0 0';
        } else {
            // Desktop: posicionar DENTRO do container relativo
            const postCard = buttonElement.closest('.post-card') || buttonElement.closest('.simple-post-card');
            
            menu.style.position = 'absolute';
            menu.style.width = `${menuWidth}px`;
            menu.style.maxWidth = `${menuWidth}px`;
            
            // Posicionar relativo ao container do post
            menu.style.top = '100%';
            menu.style.right = '0';
            menu.style.left = 'auto';
            menu.style.transform = 'none';
            menu.style.marginTop = '8px';
            
            // Se o container pai não tem position relative, adicionar
            const postMenu = buttonElement.closest('.post-menu');
            if (postMenu) {
                postMenu.style.position = 'relative';
            }
        }
    }

    closePostMenu(postId) {
        const menuId = `post-menu-${postId}`;
        const menu = document.getElementById(menuId);
        
        if (menu) {
            menu.style.display = 'none';
            menu.style.opacity = '0';
            menu.style.visibility = 'hidden';
            menu.style.pointerEvents = 'none';
        }
        
        this.activeMenus.delete(menuId);
        this.removeBackdrop(menuId);
    }

    // === MENU DE REAÇÕES === //
    toggleReactionsMenu(postId, buttonElement) {
        const menuId = `reactions-menu-${postId}`;
        const existingMenu = document.getElementById(menuId);
        
        if (existingMenu && existingMenu.style.display === 'flex') {
            this.closeReactionsMenu(postId);
            return;
        }
        
        // Fechar outros menus primeiro
        this.closeAllMenus();
        
        // Mostrar menu de reações
        this.showReactionsMenu(postId, buttonElement);
    }

    showReactionsMenu(postId, buttonElement) {
        const menuId = `reactions-menu-${postId}`;
        let menu = document.getElementById(menuId);
        
        if (!menu) {
            menu = this.createReactionsMenu(postId);
        }
        
        // Posicionar menu
        this.positionReactionsMenu(menu, buttonElement);
        
        // Mostrar menu
        menu.style.display = 'flex';
        menu.style.opacity = '1';
        menu.style.visibility = 'visible';
        menu.style.pointerEvents = 'all';
        
        this.activeMenus.add(menuId);
    }

    createReactionsMenu(postId) {
        const menu = document.createElement('div');
        menu.id = `reactions-menu-${postId}`;
        menu.className = 'reactions-menu';
        menu.style.display = 'none';
        
        const reactions = [
            { emoji: '👍', label: 'Curtir', type: 'like' },
            { emoji: '❤️', label: 'Amei', type: 'love' },
            { emoji: '😂', label: 'Divertido', type: 'laugh' },
            { emoji: '😮', label: 'Uau', type: 'wow' },
            { emoji: '😢', label: 'Triste', type: 'sad' }
        ];

        reactions.forEach(reaction => {
            const reactionButton = document.createElement('button');
            reactionButton.className = 'reaction-option';
            reactionButton.innerHTML = `
                <span class="reaction-emoji">${reaction.emoji}</span>
                <span class="reaction-label">${reaction.label}</span>
            `;
            
            reactionButton.addEventListener('click', (e) => {
                e.stopPropagation();
                this.handleReaction(postId, reaction.type, reaction.emoji);
                this.closeReactionsMenu(postId);
            });
            
            menu.appendChild(reactionButton);
        });
        
        // Adicionar ao container correto (dentro do botão like para posicionamento relativo)
        const likeButton = document.querySelector(`[data-post-id="${postId}"] .like-button`);
        if (likeButton) {
            likeButton.style.position = 'relative';
            likeButton.appendChild(menu);
        } else {
            document.body.appendChild(menu);
        }
        
        return menu;
    }

    positionReactionsMenu(menu, buttonElement) {
        const buttonRect = buttonElement.getBoundingClientRect();
        
        if (window.innerWidth <= 768) {
            // Mobile: centralizar na parte inferior
            menu.style.position = 'fixed';
            menu.style.bottom = '80px';
            menu.style.left = '50%';
            menu.style.top = 'auto';
            menu.style.transform = 'translateX(-50%)';
            menu.style.width = 'calc(100vw - 32px)';
            menu.style.maxWidth = '400px';
        } else {
            // Desktop: acima do botão
            menu.style.position = 'fixed';
            menu.style.bottom = `${window.innerHeight - buttonRect.top + 12}px`;
            menu.style.left = `${buttonRect.left + (buttonRect.width / 2)}px`;
            menu.style.top = 'auto';
            menu.style.transform = 'translateX(-50%)';
            menu.style.width = '240px';
        }
    }

    closeReactionsMenu(postId) {
        const menuId = `reactions-menu-${postId}`;
        const menu = document.getElementById(menuId);
        
        if (menu) {
            menu.style.display = 'none';
            menu.style.opacity = '0';
            menu.style.visibility = 'hidden';
            menu.style.pointerEvents = 'none';
        }
        
        this.activeMenus.delete(menuId);
    }

    // === GERENCIAMENTO GERAL === //
    closeAllMenus() {
        // Fechar menus de post
        document.querySelectorAll('.post-menu-dropdown').forEach(menu => {
            menu.style.display = 'none';
            menu.style.opacity = '0';
            menu.style.visibility = 'hidden';
            menu.style.pointerEvents = 'none';
        });
        
        // Fechar menus de reação
        document.querySelectorAll('.reactions-menu').forEach(menu => {
            menu.style.display = 'none';
            menu.style.opacity = '0';
            menu.style.visibility = 'hidden';
            menu.style.pointerEvents = 'none';
        });
        
        // Remover todos os backdrops
        document.querySelectorAll('.menu-backdrop').forEach(backdrop => {
            backdrop.remove();
        });
        
        this.activeMenus.clear();
    }

    createBackdrop(menuId) {
        const backdrop = document.createElement('div');
        backdrop.className = 'menu-backdrop';
        backdrop.id = `backdrop-${menuId}`;
        
        backdrop.addEventListener('click', () => {
            this.closeAllMenus();
        });
        
        document.body.appendChild(backdrop);
    }

    removeBackdrop(menuId) {
        const backdrop = document.getElementById(`backdrop-${menuId}`);
        if (backdrop) {
            backdrop.remove();
        }
    }

    handleDocumentClick(e) {
        // Não fechar se clicou em um menu ou botão de menu
        if (e.target.closest('.post-menu') || 
            e.target.closest('.post-menu-dropdown') ||
            e.target.closest('.reactions-menu') ||
            e.target.closest('.action-button')) {
            return;
        }
        
        this.closeAllMenus();
    }

    handleKeyDown(e) {
        if (e.key === 'Escape') {
            this.closeAllMenus();
        }
    }

    // === AÇÕES DOS MENUS === //
    async savePost(postId) {
        try {
            const response = await fetch(`/api/posts/${postId}/save`, {
                method: 'POST',
                headers: { 'Authorization': `Bearer ${localStorage.getItem('token')}` }
            });
            
            if (response.ok) {
                window.showNotification('Post salvo com sucesso!', 'success');
            }
        } catch (error) {
            window.showNotification('Erro ao salvar post', 'error');
        }
    }

    async copyPostLink(postId) {
        const url = `${window.location.origin}/post/${postId}`;
        const success = await window.copyToClipboard(url);
        
        if (success) {
            window.showNotification('Link copiado para a área de transferência!', 'success');
        } else {
            window.showNotification('Erro ao copiar link', 'error');
        }
    }

    sharePostExternal(postId) {
        window.showShareOptions(postId);
    }

    async reportPost(postId) {
        // Implementar lógica de report
        window.showNotification('Post reportado. Obrigado pelo feedback!', 'info');
    }

    async hidePost(postId) {
        const postElement = document.querySelector(`[data-post-id="${postId}"]`);
        if (postElement) {
            postElement.style.display = 'none';
            window.showNotification('Post ocultado', 'info');
        }
    }

    async unfollowAuthor(postId) {
        // Implementar lógica de unfollow
        window.showNotification('Você não segue mais este autor', 'info');
    }

    async muteNotifications(postId) {
        // Implementar lógica de mute
        window.showNotification('Notificações silenciadas para este post', 'info');
    }

    async handleReaction(postId, reactionType, emoji) {
        try {
            // Atualizar UI imediatamente
            const likeButton = document.querySelector(`[data-post-id="${postId}"] .action-button.like-button`);
            if (likeButton) {
                likeButton.classList.add('liked');
                
                // Mostrar efeito visual
                if (window.showLikeEffect) {
                    window.showLikeEffect(postId);
                }
            }

            // Enviar para API
            const response = await fetch(`/api/posts/${postId}/react`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${localStorage.getItem('token')}`
                },
                body: JSON.stringify({
                    postId: postId,
                    reactionType: reactionType
                })
            });

            if (response.ok) {
                window.showNotification(`Reagiu com ${emoji}`, 'success');
            }
        } catch (error) {
            console.error('Erro ao reagir:', error);
            window.showNotification('Erro ao processar reação', 'error');
        }
    }
}

// Inicializar o gerenciador de menus
const feedMenuManager = new FeedMenuManager();

// Expor funções globais para o Blazor
window.togglePostMenu = (postId, buttonElement) => {
    feedMenuManager.togglePostMenu(postId, buttonElement);
};

window.toggleReactionsMenu = (postId, buttonElement) => {
    feedMenuManager.toggleReactionsMenu(postId, buttonElement);
};

window.closeAllMenus = () => {
    feedMenuManager.closeAllMenus();
};

console.log('Feed Menu Manager carregado com sucesso!');