class AdminImageUploader {
    constructor(options = {}) {
        this.fileInputId = options.fileInputId || 'imageFile';
        this.previewContainerId = options.previewContainerId || 'imagePreview';
        this.imageUrlInputId = options.imageUrlInputId || 'imageUrl';
        this.dropZoneId = options.dropZoneId || 'dropZone';
        this.progressBarId = options.progressBarId || 'uploadProgress';
        this.eventIdInputId = options.eventIdInputId || 'eventId';

        this.init();
    }

    init() {
        this.fileInput = document.getElementById(this.fileInputId);
        this.imageUrlInput = document.getElementById(this.imageUrlInputId);
        this.previewContainer = document.getElementById(this.previewContainerId);
        this.dropZone = document.getElementById(this.dropZoneId);
        this.progressBar = document.getElementById(this.progressBarId);

        if (this.fileInput) {
            this.fileInput.addEventListener('change', (e) =>
                this.handleFileSelect(e));
        }

        if (this.dropZone) {
            this.setupDragAndDrop();
        }
    }

    setupDragAndDrop() {
        this.dropZone.addEventListener('dragover', (e) => {
            e.preventDefault();
            this.dropZone.classList.add('drag-over');
        });

        this.dropZone.addEventListener('dragleave', () => {
            this.dropZone.classList.remove('drag-over');
        });

        this.dropZone.addEventListener('drop', (e) => {
            e.preventDefault();
            this.dropZone.classList.remove('drag-over');

            const files = e.dataTransfer.files;
            if (files.length > 0) {
                this.fileInput.files = files;
                this.handleFileSelect({ target: this.fileInput });
            }
        });
    }

    handleFileSelect(event) {
        const file = event.target.files[0];
        if (!file) return;

        this.showPreview(file);
        this.uploadImage(file);
    }

    showPreview(file) {
        const reader = new FileReader();

        reader.onload = (e) => {
            if (this.previewContainer) {
                this.previewContainer.innerHTML = `
                    <div class="preview-wrapper">
                        <img src="${e.target.result}" 
                             alt="Preview" 
                             class="preview-image">
                        <small class="preview-info">
                            ${(file.size / 1024 / 1024).toFixed(2)}MB
                        </small>
                    </div>
                `;
                this.previewContainer.style.display = 'block';
            }
        };

        reader.readAsDataURL(file);
    }

    async uploadImage(file) {
        let eventId = 0;

        // First try: Find by NAME="Id"
        let eventIdInput = document.querySelector('input[name="Id"]');
        if (eventIdInput && eventIdInput.value) {
            const val = parseInt(eventIdInput.value);
            if (val > 0) {
                eventId = val;
            }
        }

        // Second try: Find GUID
        if (!eventId) {
            const guidPattern =
                /^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i;
            const allInputs = document.querySelectorAll(
                'input[type="hidden"]');

            for (let input of allInputs) {
                if (guidPattern.test(input.value)) {
                    eventId = input.value;
                    break;
                }
            }
        }

        if (!eventId) {
            this.showError(
                'Event ID is required. Save event first.');
            return;
        }

        const formData = new FormData();
        formData.append('file', file);
        formData.append('eventId', eventId);

        this.showProgress(true);

        try {
            const response = await fetch('/api/eventimages/upload', {
                method: 'POST',
                body: formData
            });

            if (!response.ok) {
                const contentType = response.headers.get('content-type');
                let errorText = '';

                if (contentType &&
                    contentType.includes('application/json')) {
                    const error = await response.json();
                    errorText = error.message || JSON.stringify(error);
                } else {
                    errorText = await response.text();
                }

                throw new Error(
                    `HTTP ${response.status}: ${errorText}`);
            }

            const data = await response.json();

            if (this.imageUrlInput) {
                this.imageUrlInput.value =
                    data.originalImageUrl;
            }

            // Also store thumbnail URL if there's a field for it
            const thumbnailInput = document.getElementById('ThumbnailUrl');
            if (thumbnailInput && data.thumbnailImageUrl) {
                thumbnailInput.value = data.thumbnailImageUrl;
            }

            this.showSuccess('Image uploaded successfully');
            this.showProgress(false);
        } catch (error) {
            this.showError(error.message ||
                'Failed to upload image');
            this.showProgress(false);
        }
    }

    showProgress(show) {
        if (this.progressBar) {
            this.progressBar.style.display = show ? 'block' : 'none';
        }
    }

    showSuccess(message) {
        this.showMessage(message, 'success');
    }

    showError(message) {
        this.showMessage(message, 'danger');
    }

    showMessage(message, type = 'info') {
        const alertDiv = document.createElement('div');
        alertDiv.className = `alert alert-${type} alert-dismissible fade show`;
        alertDiv.style.zIndex = '9999';
        alertDiv.style.position = 'fixed';
        alertDiv.style.top = '20px';
        alertDiv.style.right = '20px';
        alertDiv.style.minWidth = '300px';
        alertDiv.innerHTML = `
            ${message}
            <button type="button" class="btn-close" 
                    data-bs-dismiss="alert"></button>
        `;

        document.body.appendChild(alertDiv);

        setTimeout(() => {
            alertDiv.remove();
        }, 5000);
    }

    getCsrfToken() {
        const token = document.querySelector(
            'input[name="__RequestVerificationToken"]');
        return token ? token.value : '';
    }
}

document.addEventListener('DOMContentLoaded', () => {
    if (document.getElementById('imageFile')) {
        new AdminImageUploader({
            fileInputId: 'imageFile',
            previewContainerId: 'imagePreview',
            imageUrlInputId: 'ImageUrl',
            dropZoneId: 'dropZone',
            progressBarId: 'uploadProgress',
            eventIdInputId: 'Id'
        });
    }
});
