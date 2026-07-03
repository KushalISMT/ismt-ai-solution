(function () {
    'use strict';

    const widget = document.getElementById('ai-chatbot');
    if (!widget) return;

    const processUrl = widget.dataset.processUrl || '/Chatbot/Process';
    const chatWindow = widget.querySelector('.ai-chat-window');
    const toggleBtn = widget.querySelector('.ai-chat-toggle');
    const closeBtn = widget.querySelector('.ai-chat-close');
    const form = widget.querySelector('.ai-chat-form');
    const input = widget.querySelector('.ai-chat-input');
    const messagesEl = widget.querySelector('.ai-chat-messages');
    const typingEl = widget.querySelector('.ai-chat-typing');
    const quickRepliesEl = widget.querySelector('.ai-chat-quick-replies');

    const STORAGE_KEY = 'ai-chatbot-history';
    const OPEN_KEY = 'ai-chatbot-open';
    const SESSION_KEY = 'ai-chatbot-session';
    const MAX_HISTORY = 50;

    if (!sessionStorage.getItem(SESSION_KEY)) {
        sessionStorage.setItem(SESSION_KEY, crypto.randomUUID ? crypto.randomUUID() : Date.now().toString());
    }

    const clientIntents = [
        {
            id: 'greeting',
            keywords: ['hello', 'hi', 'hey', 'greetings', 'good morning', 'good afternoon', 'good evening', 'howdy'],
            response: 'Hello! Welcome to AI-Solutions. How can I help you today?',
            suggestions: [
                { label: 'Solutions', type: 'quick', value: 'What solutions do you offer?' },
                { label: 'Case Studies', type: 'quick', value: 'Show me case studies' },
                { label: 'Contact Us', type: 'link', value: '/Contact' }
            ]
        },
        {
            id: 'help',
            keywords: ['help', 'what can you do', 'support', 'assist'],
            response: 'I can help you explore our solutions, case studies, articles, events, feedback, or contact form. Just type your question or use the quick replies below.',
            suggestions: [
                { label: 'Solutions', type: 'quick', value: 'What solutions do you offer?' },
                { label: 'Articles', type: 'quick', value: 'Latest articles' },
                { label: 'Contact Us', type: 'link', value: '/Contact' }
            ]
        },
        {
            id: 'thanks',
            keywords: ['thanks', 'thank you', 'thx', 'appreciate'],
            response: `You're welcome! Let me know if there's anything else I can help with.`,
            suggestions: [
                { label: 'Close chat', type: 'quick', value: 'goodbye' }
            ]
        },
        {
            id: 'goodbye',
            keywords: ['bye', 'goodbye', 'see you', 'later', 'exit', 'close'],
            response: 'Goodbye! Feel free to come back anytime. Have a great day!',
            suggestions: []
        },
        {
            id: 'howareyou',
            keywords: ['how are you', 'how is it going', 'how do you do'],
            response: `I'm a bot, but I'm running at full speed and ready to help you explore AI-Solutions!`,
            suggestions: [
                { label: 'Help', type: 'quick', value: 'help' }
            ]
        },
        {
            id: 'name',
            keywords: ['your name', 'who are you'],
            response: `I'm the AI-Solutions virtual assistant. I'm here to answer questions about our company, solutions, and services.`,
            suggestions: [
                { label: 'Help', type: 'quick', value: 'help' }
            ]
        }
    ];

    function escapeHtml(text) {
        const div = document.createElement('div');
        div.textContent = text;
        return div.innerHTML;
    }

    function normalize(text) {
        return text.toLowerCase().replace(/[?.,!;:]+$/, '').trim();
    }

    function findClientIntent(text) {
        const normalized = normalize(text);
        let best = null;
        let bestScore = 0;

        for (const intent of clientIntents) {
            let score = 0;
            for (const keyword of intent.keywords) {
                if (normalized === keyword || normalized.includes(keyword)) {
                    score += keyword.split(' ').length;
                }
            }
            if (score > bestScore) {
                bestScore = score;
                best = intent;
            }
        }

        return bestScore > 0 ? best : null;
    }

    function saveHistory() {
        try {
            const items = [];
            const bubbles = messagesEl.querySelectorAll('.ai-chat-message');
            bubbles.forEach(b => {
                items.push({
                    sender: b.dataset.sender,
                    html: b.innerHTML
                });
            });
            while (items.length > MAX_HISTORY) items.shift();
            localStorage.setItem(STORAGE_KEY, JSON.stringify(items));
        } catch {
            // ignore storage errors
        }
    }

    function loadHistory() {
        try {
            const raw = localStorage.getItem(STORAGE_KEY);
            if (!raw) return false;
            const items = JSON.parse(raw);
            if (!Array.isArray(items) || items.length === 0) return false;
            items.forEach(item => appendMessageHtml(item.html, item.sender));
            return true;
        } catch {
            return false;
        }
    }

    function appendMessageHtml(html, sender) {
        const div = document.createElement('div');
        div.className = `ai-chat-message ai-chat-message-${sender}`;
        div.dataset.sender = sender;
        div.innerHTML = html;
        messagesEl.appendChild(div);
        scrollToBottom();
    }

    function addUserMessage(text) {
        appendMessageHtml(escapeHtml(text), 'user');
        saveHistory();
    }

    function addBotMessage(html, suggestions) {
        const time = new Date().toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
        const bubbleHtml = `<div class="ai-chat-bubble">${html}</div><div class="ai-chat-time">${escapeHtml(time)}</div>`;
        appendMessageHtml(bubbleHtml, 'bot');
        saveHistory();
        renderSuggestions(suggestions);
    }

    function showTyping() {
        typingEl.style.display = 'flex';
        scrollToBottom();
    }

    function hideTyping() {
        typingEl.style.display = 'none';
    }

    function renderSuggestions(suggestions) {
        quickRepliesEl.innerHTML = '';
        if (!suggestions || suggestions.length === 0) return;

        suggestions.forEach(s => {
            const btn = document.createElement('button');
            btn.type = 'button';
            btn.className = 'ai-chat-chip';
            btn.textContent = s.label;
            btn.addEventListener('click', () => {
                if (s.type === 'link') {
                    window.location.href = s.value;
                } else {
                    sendMessage(s.value);
                }
            });
            quickRepliesEl.appendChild(btn);
        });
    }

    function scrollToBottom() {
        messagesEl.scrollTop = messagesEl.scrollHeight;
    }

    function setOpen(isOpen) {
        if (isOpen) {
            chatWindow.classList.add('open');
            toggleBtn.setAttribute('aria-expanded', 'true');
            input.focus();
        } else {
            chatWindow.classList.remove('open');
            toggleBtn.setAttribute('aria-expanded', 'false');
        }
        try {
            localStorage.setItem(OPEN_KEY, isOpen ? '1' : '0');
        } catch {
            // ignore
        }
    }

    function toggleChat() {
        setOpen(!chatWindow.classList.contains('open'));
    }

    async function handleServerFallback(text) {
        try {
            const response = await fetch(processUrl, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json', 'Accept': 'application/json' },
                body: JSON.stringify({ message: text })
            });

            if (!response.ok) {
                throw new Error(`Server returned ${response.status}`);
            }

            const data = await response.json();
            addBotMessage(data.reply || `I'm not sure how to answer that.`, data.suggestions);
        } catch (error) {
            console.error('Chatbot server fallback failed:', error);
            addBotMessage(
                `I'm having trouble connecting to my knowledge base right now. You can still reach us through the Contact Us form.`,
                [{ label: 'Contact Us', type: 'link', value: '/Contact' }]
            );
        }
    }

    async function sendMessage(text) {
        const trimmed = text.trim();
        if (!trimmed) return;

        input.value = '';
        renderSuggestions([]);
        addUserMessage(trimmed);

        const intent = findClientIntent(trimmed);
        if (intent) {
            showTyping();
            await delay(400 + Math.random() * 400);
            hideTyping();
            addBotMessage(intent.response, intent.suggestions);

            if (intent.id === 'goodbye') {
                setTimeout(() => setOpen(false), 1500);
            }
            return;
        }

        showTyping();
        await handleServerFallback(trimmed);
        hideTyping();
    }

    function delay(ms) {
        return new Promise(resolve => setTimeout(resolve, ms));
    }

    function init() {
        try {
            const open = localStorage.getItem(OPEN_KEY) === '1';
            if (open) setOpen(true);
        } catch {
            // ignore
        }

        const hasHistory = loadHistory();
        if (!hasHistory) {
            addBotMessage(
                `Hi there! I'm the AI-Solutions assistant. How can I help you today?`,
                [
                    { label: 'Solutions', type: 'quick', value: 'What solutions do you offer?' },
                    { label: 'Case Studies', type: 'quick', value: 'Show me case studies' },
                    { label: 'Contact Us', type: 'link', value: '/Contact' }
                ]
            );
        }

        toggleBtn.addEventListener('click', toggleChat);
        closeBtn.addEventListener('click', () => setOpen(false));
        form.addEventListener('submit', e => {
            e.preventDefault();
            sendMessage(input.value);
        });
    }

    init();
})();
