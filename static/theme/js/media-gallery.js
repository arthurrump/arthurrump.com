/**
 * Media Gallery — modal viewer and alt-text toggle.
 *
 * Dynamically creates all modal DOM elements so pages without a gallery
 * carry zero gallery-related markup. Loaded only when the media-gallery
 * partial is rendered.
 */
(function () {
  'use strict';

  // --- DOM creation --------------------------------------------------------

  function createModal() {
    var modal = document.createElement('div');
    modal.className = 'media-modal';
    modal.setAttribute('aria-hidden', 'true');

    var closeBtn = document.createElement('button');
    closeBtn.className = 'media-modal-close';
    closeBtn.setAttribute('aria-label', 'Close modal');
    closeBtn.innerHTML = '&times;';

    var container = document.createElement('div');
    container.className = 'media-modal-img-container';

    var img = document.createElement('img');
    img.className = 'media-modal-content';
    img.id = 'global-media-modal-img';
    img.alt = '';
    img.style.display = 'none';

    var video = document.createElement('video');
    video.className = 'media-modal-content';
    video.id = 'global-media-modal-video';
    video.controls = true;
    video.style.display = 'none';

    var altBtn = document.createElement('button');
    altBtn.id = 'global-media-modal-alt-btn';
    altBtn.className = 'alt-btn';
    altBtn.type = 'button';
    altBtn.setAttribute('aria-label', 'Show alt text');
    altBtn.setAttribute('title', 'Show alt text');
    altBtn.textContent = 'ALT';
    altBtn.style.display = 'none';

    var altOverlay = document.createElement('div');
    altOverlay.className = 'alt-text-overlay';
    altOverlay.id = 'global-media-modal-alt';

    container.appendChild(img);
    container.appendChild(video);
    container.appendChild(altBtn);
    container.appendChild(altOverlay);
    modal.appendChild(closeBtn);
    modal.appendChild(container);
    document.body.appendChild(modal);

    return {
      modal: modal,
      img: img,
      video: video,
      altBtn: altBtn,
      altOverlay: altOverlay,
      closeBtn: closeBtn,
      container: container,
    };
  }

  // --- Init ----------------------------------------------------------------

  function init() {
    // Bail out if there's no gallery on the page.
    if (!document.querySelector('.media-gallery')) return;

    var ui = createModal();

    var modal = ui.modal;
    var modalImg = ui.img;
    var modalVideo = ui.video;
    var modalAlt = ui.altOverlay;
    var modalAltBtn = ui.altBtn;
    var closeBtn = ui.closeBtn;

    function closeModal() {
      modal.classList.remove('open');
      modal.setAttribute('aria-hidden', 'true');
      modalVideo.pause();
      modalImg.src = '';
      modalVideo.removeAttribute('src');
      modalVideo.load();
      modalImg.style.display = 'none';
      modalVideo.style.display = 'none';
      modalAlt.classList.remove('visible');
      modalAltBtn.style.display = 'none';
      document.body.style.overflow = '';
    }

    // Open modal on media click (delegated, works for dynamically added galleries)
    document.body.addEventListener('click', function (e) {
      var target = e.target;
      if (!target.matches('.media-image') && !target.matches('.media-video')) return;

      var fullSrc = target.getAttribute('data-full-src');
      var alt = target.getAttribute('alt');
      var isVideo = target.matches('.media-video');

      if (isVideo) {
        modalVideo.src = fullSrc;
        modalVideo.style.display = 'block';
        modalVideo.play().catch(function () {});
      } else {
        modalImg.src = fullSrc;
        modalImg.style.display = 'block';
      }

      modalImg.alt = alt || '';
      modalAlt.textContent = alt || '';
      modalAltBtn.style.display = alt ? 'block' : 'none';

      modal.classList.add('open');
      modal.setAttribute('aria-hidden', 'false');
      document.body.style.overflow = 'hidden';
    });

    closeBtn.addEventListener('click', closeModal);

    modal.addEventListener('click', function (e) {
      // Close if clicking outside the media container, or on the image itself.
      // Clicking the video or alt-text controls should NOT close.
      if (e.target === modal || e.target === modalImg || e.target.closest('.media-modal-img-container') === null) {
        closeModal();
      }
    });

    document.addEventListener('keydown', function (e) {
      if (e.key === 'Escape' && modal.classList.contains('open')) closeModal();
    });

    // Alt-text toggle (delegated, covers both grid items and modal button)
    document.body.addEventListener('click', function (e) {
      if (!e.target.matches('.alt-btn')) return;
      e.preventDefault();
      e.stopPropagation();
      var overlay = e.target.nextElementSibling;
      if (overlay && overlay.classList.contains('alt-text-overlay')) {
        overlay.classList.toggle('visible');
      }
    });
  }

  if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', init);
  } else {
    init();
  }
})();