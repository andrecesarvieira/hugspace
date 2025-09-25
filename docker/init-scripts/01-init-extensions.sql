-- Script de inicialização do PostgreSQL para HugSpace
-- Cria extensões necessárias para rede social

-- Extensões para busca full-text e performance
CREATE EXTENSION IF NOT EXISTS "pg_trgm";      -- Busca trigram para autocomplete
CREATE EXTENSION IF NOT EXISTS "unaccent";     -- Remove acentos para busca
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";    -- UUIDs otimizados
CREATE EXTENSION IF NOT EXISTS "pgcrypto";     -- Funções de criptografia

-- Configurar encoding para suporte completo a emojis
SET client_encoding = 'UTF8';

-- Criar schemas organizacionais (opcional - DDD)
CREATE SCHEMA IF NOT EXISTS users;
CREATE SCHEMA IF NOT EXISTS social;
CREATE SCHEMA IF NOT EXISTS content;
CREATE SCHEMA IF NOT EXISTS notifications;

-- Logs de inicialização
\echo 'HugSpace: Extensões PostgreSQL configuradas com sucesso!'
\echo 'Extensões disponíveis: pg_trgm, unaccent, uuid-ossp, pgcrypto'
\echo 'Schemas criados: users, social, content, notifications'