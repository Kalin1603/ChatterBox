document.addEventListener('DOMContentLoaded', function () {
    document.querySelectorAll('.like-count').forEach(function (likeCountElement) {
        const postId = likeCountElement.dataset.postId;
        const tooltip = document.getElementById('tooltip-' + postId);
        if (tooltip) {
            likeCountElement.addEventListener('click', function (e) {
                e.stopPropagation();
                tooltip.classList.toggle('hidden');
            });
            document.addEventListener('click', function () {
                tooltip.classList.add('hidden');
            });
            tooltip.addEventListener('click', function (e) {
                e.stopPropagation();
            });
        }
    });
});