window.showRecipeModal = () => {
    const modal = new bootstrap.Modal(document.getElementById('recipeModal'));
    modal.show();
};
window.hideModal = (modalId) => {
    const modalElement = document.getElementById(modalId);
    if (!modalElement) return;

    const modal = bootstrap.Modal.getInstance(modalElement);
    if (modal) modal.hide();
};
window.showAchievementModal = () => {
    const modalEl = document.getElementById("achievementDetailModal");
    if (!modalEl) {
        console.error("Modal DOM element not found");
        return;
    }

    const modal = new bootstrap.Modal(modalEl);
    modal.show();
};
window.showConfirmModal = () => {
    const confirmElement = document.getElementById("confirmModal")
    if (!confirmElement) {
        console.error("Confirm Modal DOM element not found");
        return;
    }
    const modal = new bootstrap.Modal(confirmElement);
    modal.show();
}