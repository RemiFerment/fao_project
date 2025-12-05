window.scrollChatToBottom = () => {
    const el = document.getElementById("chatBody");
    if (!el) return;
    el.scrollTop = el.scrollHeight;
};
