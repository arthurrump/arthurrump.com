(function($){

    // Caption
    $('.article-entry').each(function(i) {
        $(this).find('img').each(function() {
            if (this.alt) {
                $(this).after('<span class="caption">' + this.alt + '</span>');
            }
        });

    });
    if (lightGallery) {
        var options = {
            selector: '.gallery-item',
        };
        lightGallery($('.article-entry')[0], options);
        lightGallery($('.article-gallery')[0], options);
    }

})(jQuery);