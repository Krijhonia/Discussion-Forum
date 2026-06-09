/**
 * Tiptap Rich Text Editor - Discussion Forum
 * A free, open-source rich text editor with no watermarks.
 * Uses contentEditable with custom toolbar for rich formatting.
 */

function initTiptapEditor(editorId, toolbarId, hiddenInputId) {
    const editorEl = document.getElementById(editorId);
    const toolbarEl = document.getElementById(toolbarId);
    const hiddenInput = document.getElementById(hiddenInputId);

    if (!editorEl || !toolbarEl || !hiddenInput) {
        console.error('Tiptap Editor: Missing required elements', { editorId, toolbarId, hiddenInputId });
        return;
    }

    // Make editor contenteditable
    editorEl.setAttribute('contenteditable', 'true');
    editorEl.setAttribute('role', 'textbox');
    editorEl.setAttribute('aria-multiline', 'true');
    editorEl.setAttribute('data-placeholder', 'Write your message...');

    // Load existing content from the hidden input (for edit pages)
    if (hiddenInput.value && hiddenInput.value.trim() !== '') {
        editorEl.innerHTML = hiddenInput.value;
    }

    // Sync content to hidden input on every change
    function syncContent() {
        hiddenInput.value = editorEl.innerHTML;
    }

    editorEl.addEventListener('input', syncContent);
    editorEl.addEventListener('keyup', syncContent);

    // Also sync before form submission
    const form = hiddenInput.closest('form');
    if (form) {
        form.addEventListener('submit', function () {
            syncContent();
        });
    }

    // Track active state of buttons
    function updateToolbarState() {
        const buttons = toolbarEl.querySelectorAll('.tiptap-btn');
        buttons.forEach(function (btn) {
            const action = btn.getAttribute('data-action');
            let isActive = false;

            switch (action) {
                case 'bold':
                    isActive = document.queryCommandState('bold');
                    break;
                case 'italic':
                    isActive = document.queryCommandState('italic');
                    break;
                case 'underline':
                    isActive = document.queryCommandState('underline');
                    break;
                case 'strike':
                    isActive = document.queryCommandState('strikethrough');
                    break;
                case 'bulletList':
                    isActive = document.queryCommandState('insertUnorderedList');
                    break;
                case 'orderedList':
                    isActive = document.queryCommandState('insertOrderedList');
                    break;
            }

            btn.classList.toggle('is-active', isActive);
        });

        // Update heading select
        const headingSelect = toolbarEl.querySelector('[data-action="heading"]');
        if (headingSelect) {
            const sel = window.getSelection();
            if (sel && sel.rangeCount > 0) {
                let node = sel.anchorNode;
                if (node && node.nodeType === 3) node = node.parentNode;

                let headingLevel = '0';
                while (node && node !== editorEl) {
                    const tag = node.tagName;
                    if (tag && /^H[1-3]$/.test(tag)) {
                        headingLevel = tag.charAt(1);
                        break;
                    }
                    node = node.parentNode;
                }
                headingSelect.value = headingLevel;
            }
        }
    }

    editorEl.addEventListener('keyup', updateToolbarState);
    editorEl.addEventListener('mouseup', updateToolbarState);
    editorEl.addEventListener('click', updateToolbarState);

    // Toolbar button actions
    toolbarEl.addEventListener('click', function (e) {
        const btn = e.target.closest('.tiptap-btn');
        if (!btn) return;

        e.preventDefault();
        editorEl.focus();

        const action = btn.getAttribute('data-action');

        switch (action) {
            case 'bold':
                document.execCommand('bold', false, null);
                break;
            case 'italic':
                document.execCommand('italic', false, null);
                break;
            case 'underline':
                document.execCommand('underline', false, null);
                break;
            case 'strike':
                document.execCommand('strikethrough', false, null);
                break;
            case 'alignLeft':
                document.execCommand('justifyLeft', false, null);
                break;
            case 'alignCenter':
                document.execCommand('justifyCenter', false, null);
                break;
            case 'alignRight':
                document.execCommand('justifyRight', false, null);
                break;
            case 'bulletList':
                document.execCommand('insertUnorderedList', false, null);
                break;
            case 'orderedList':
                document.execCommand('insertOrderedList', false, null);
                break;
            case 'blockquote':
                toggleBlockquote();
                break;
            case 'codeBlock':
                insertCodeBlock();
                break;
            case 'link':
                insertLink();
                break;
            case 'clearFormat':
                document.execCommand('removeFormat', false, null);
                // Also remove block-level formatting
                document.execCommand('formatBlock', false, 'p');
                break;
            case 'undo':
                document.execCommand('undo', false, null);
                break;
            case 'redo':
                document.execCommand('redo', false, null);
                break;
        }

        syncContent();
        updateToolbarState();
    });

    // Heading select handler
    const headingSelect = toolbarEl.querySelector('[data-action="heading"]');
    if (headingSelect) {
        headingSelect.addEventListener('change', function () {
            editorEl.focus();
            const level = this.value;
            if (level === '0') {
                document.execCommand('formatBlock', false, 'p');
            } else {
                document.execCommand('formatBlock', false, 'h' + level);
            }
            syncContent();
        });
    }

    // Code block insertion
    function insertCodeBlock() {
        const sel = window.getSelection();
        if (!sel || sel.rangeCount === 0) return;

        const range = sel.getRangeAt(0);
        const selectedText = range.toString();

        const pre = document.createElement('pre');
        const code = document.createElement('code');
        code.textContent = selectedText || 'Enter your code here...';
        pre.appendChild(code);

        range.deleteContents();
        range.insertNode(pre);

        // Move cursor after the code block
        const newRange = document.createRange();
        newRange.setStartAfter(pre);
        newRange.collapse(true);
        sel.removeAllRanges();
        sel.addRange(newRange);

        syncContent();
    }

    // Blockquote toggle
    function toggleBlockquote() {
        const sel = window.getSelection();
        if (!sel || sel.rangeCount === 0) return;

        let node = sel.anchorNode;
        if (node && node.nodeType === 3) node = node.parentNode;

        // Check if we're inside a blockquote
        let bq = null;
        let check = node;
        while (check && check !== editorEl) {
            if (check.tagName === 'BLOCKQUOTE') {
                bq = check;
                break;
            }
            check = check.parentNode;
        }

        if (bq) {
            // Remove blockquote – unwrap it
            const parent = bq.parentNode;
            while (bq.firstChild) {
                parent.insertBefore(bq.firstChild, bq);
            }
            parent.removeChild(bq);
        } else {
            document.execCommand('formatBlock', false, 'blockquote');
        }

        syncContent();
    }

    // Link insertion
    function insertLink() {
        const sel = window.getSelection();
        const selectedText = sel ? sel.toString() : '';

        // Check if cursor is inside a link
        let linkNode = null;
        if (sel && sel.rangeCount > 0) {
            let node = sel.anchorNode;
            if (node && node.nodeType === 3) node = node.parentNode;
            while (node && node !== editorEl) {
                if (node.tagName === 'A') {
                    linkNode = node;
                    break;
                }
                node = node.parentNode;
            }
        }

        if (linkNode) {
            // Unlink
            document.execCommand('unlink', false, null);
        } else {
            const url = prompt('Enter URL:', 'https://');
            if (url) {
                if (selectedText) {
                    document.execCommand('createLink', false, url);
                } else {
                    const a = document.createElement('a');
                    a.href = url;
                    a.textContent = url;
                    a.target = '_blank';

                    const range = sel.getRangeAt(0);
                    range.insertNode(a);

                    // Move cursor after the link
                    const newRange = document.createRange();
                    newRange.setStartAfter(a);
                    newRange.collapse(true);
                    sel.removeAllRanges();
                    sel.addRange(newRange);
                }
            }
        }

        syncContent();
    }

    // Handle Tab key inside code blocks
    editorEl.addEventListener('keydown', function (e) {
        if (e.key === 'Tab') {
            const sel = window.getSelection();
            if (sel && sel.rangeCount > 0) {
                let node = sel.anchorNode;
                if (node && node.nodeType === 3) node = node.parentNode;

                // Check if inside a <pre> or <code>
                let inCode = false;
                let check = node;
                while (check && check !== editorEl) {
                    if (check.tagName === 'PRE' || check.tagName === 'CODE') {
                        inCode = true;
                        break;
                    }
                    check = check.parentNode;
                }

                if (inCode) {
                    e.preventDefault();
                    document.execCommand('insertText', false, '    ');
                    syncContent();
                }
            }
        }
    });

    // Handle paste – clean up pasted HTML
    editorEl.addEventListener('paste', function (e) {
        // Let the browser handle it but sync after
        setTimeout(syncContent, 10);
    });

    // Initial sync
    syncContent();
}
