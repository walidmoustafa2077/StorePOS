/**
 * StorePOS API Documentation - Consolidated Application Script
 * 
 * This file consolidates all JavaScript functionality for the StorePOS API documentation:
 * - Guide functionality (theme switching, search, navigation)
 * - Example loading and management
 * - Section configuration
 * - Language switching and code highlighting
 * - API testing utilities
 */

// =============================================================================
// CONFIGURATION
// =============================================================================

/**
 * Section Configuration for StorePOS API Documentation
 * Defines the structure and metadata for each documentation section
 */
const SECTIONS_CONFIG = {
    'getting-started': {
        title: '<i class="fas fa-rocket mr-3"></i>Getting Started',
        description: 'Learn the basics of working with the StorePOS API, including setup, basic requests, and authentication flow.',
        languages: ['powershell', 'csharp', 'javascript', 'python']
    },
    'overview': {
        title: '<i class="fas fa-list-alt mr-3"></i>API Overview',
        description: 'Understand the StorePOS API structure, endpoints, response formats, and general usage patterns.',
        languages: ['powershell', 'csharp', 'javascript', 'python']
    },
    'authentication': {
        title: '<i class="fas fa-lock mr-3"></i>Authentication',
        description: 'Learn how to authenticate with the StorePOS API using JWT tokens, manage user sessions, and handle token refresh.',
        languages: ['powershell', 'csharp', 'javascript', 'python']
    },
    'products': {
        title: '<i class="fas fa-box mr-3"></i>Products Management',
        description: 'Manage products, inventory, categories, and stock levels using the StorePOS API endpoints.',
        languages: ['powershell', 'csharp', 'javascript', 'python']
    },
    'sales': {
        title: '<i class="fas fa-cash-register mr-3"></i>Sales & Transactions',
        description: 'Process sales, manage shopping carts, handle transactions, and work with different payment methods.',
        languages: ['powershell', 'csharp', 'javascript', 'python']
    },
    'users': {
        title: '<i class="fas fa-users mr-3"></i>User Management',
        description: 'Manage users, roles, permissions, and user authentication within the StorePOS system.',
        languages: ['powershell', 'csharp', 'javascript', 'python']
    }
};

const LANGUAGE_CONFIG = {
    'powershell': {
        name: 'PowerShell',
        icon: 'PS',
        bgClass: 'bg-primary-100 dark:bg-primary-900',
        textClass: 'text-primary-600 dark:text-primary-400',
        description: 'Native Windows scripting with Invoke-RestMethod for easy API integration.',
        extension: 'ps1'
    },
    'csharp': {
        name: 'C# / .NET',
        icon: 'C#',
        bgClass: 'bg-purple-100 dark:bg-purple-900',
        textClass: 'text-purple-600 dark:text-purple-400',
        description: 'Modern .NET HttpClient with System.Text.Json for high-performance applications.',
        extension: 'cs'
    },
    'javascript': {
        name: 'JavaScript',
        icon: 'JS',
        bgClass: 'bg-yellow-100 dark:bg-yellow-900',
        textClass: 'text-yellow-600 dark:text-yellow-400',
        description: 'Works in both Node.js and browsers with modern fetch API and async/await.',
        extension: 'js'
    },
    'python': {
        name: 'Python',
        icon: 'PY',
        bgClass: 'bg-accent-100 dark:bg-accent-900',
        textClass: 'text-accent-600 dark:text-accent-400',
        description: 'Clean and simple implementation using requests library with type hints.',
        extension: 'py'
    },
    'java': {
        name: 'Java',
        icon: 'JV',
        bgClass: 'bg-red-100 dark:bg-red-900',
        textClass: 'text-red-600 dark:text-red-400',
        description: 'Enterprise-ready implementation using HttpClient (Java 11+) with Jackson.',
        extension: 'java'
    },
    'go': {
        name: 'Go',
        icon: 'GO',
        bgClass: 'bg-cyan-100 dark:bg-cyan-900',
        textClass: 'text-cyan-600 dark:text-cyan-400',
        description: 'High-performance implementation using standard net/http package.',
        extension: 'go'
    }
};

// =============================================================================
// EXAMPLE LOADER CLASS
// =============================================================================

class ExampleLoader {
    constructor() {
        this.baseUrl = './examples';
        this.cache = new Map();
    }

    /**
     * Load example code from file
     * @param {string} language - Programming language
     * @param {string} section - Section name
     * @returns {Promise<string>} - Code content
     */
    async loadExample(language, section) {
        const cacheKey = `${language}-${section}`;
        
        if (this.cache.has(cacheKey)) {
            return this.cache.get(cacheKey);
        }

        try {
            const extension = LANGUAGE_CONFIG[language]?.extension;
            if (!extension) {
                throw new Error(`Unknown language: ${language}`);
            }

            const url = `${this.baseUrl}/${language}/${section}.${extension}`;
            const response = await fetch(url);
            
            if (!response.ok) {
                throw new Error(`Failed to load ${url}: ${response.status}`);
            }

            const code = await response.text();
            this.cache.set(cacheKey, code);
            return code;
        } catch (error) {
            console.warn(`Failed to load example for ${language}/${section}:`, error);
            return this.getFallbackCode(language, section);
        }
    }

    /**
     * Get fallback code when file loading fails
     */
    getFallbackCode(language, section) {
        const langConfig = LANGUAGE_CONFIG[language];
        const langName = langConfig?.name || language;
        const sectionTitle = section.charAt(0).toUpperCase() + section.slice(1).replace('-', ' ');
        
        const fallbacks = {
            'powershell': `# ${sectionTitle} example for ${langName}\n# Example file not found. Please check the file path.\n\nWrite-Host "Example coming soon..."`,
            'csharp': `// ${sectionTitle} example for ${langName}\n// Example file not found. Please check the file path.\n\nConsole.WriteLine("Example coming soon...");`,
            'javascript': `// ${sectionTitle} example for ${langName}\n// Example file not found. Please check the file path.\n\nconsole.log("Example coming soon...");`,
            'python': `# ${sectionTitle} example for ${langName}\n# Example file not found. Please check the file path.\n\nprint("Example coming soon...")`,
            'java': `// ${sectionTitle} example for ${langName}\n// Example file not found. Please check the file path.\n\nSystem.out.println("Example coming soon...");`,
            'go': `// ${sectionTitle} example for ${langName}\n// Example file not found. Please check the file path.\n\nfmt.Println("Example coming soon...")`
        };
        
        return fallbacks[language] || `# Example for ${language}/${section} not found`;
    }

    /**
     * Generate HTML for language tabs
     */
    generateLanguageTabs(sectionId, languages) {
        return languages.map((lang, index) => {
            const isActive = index === 0;
            const langConfig = LANGUAGE_CONFIG[lang];
            const activeClass = isActive ? 'border-primary-500 text-primary-600 dark:text-primary-400' : 'border-transparent text-secondary-500 hover:text-secondary-700 dark:text-secondary-400 dark:hover:text-secondary-200';
            
            return `
                <button onclick="showLanguageTab('${sectionId}', '${lang}')" 
                    class="language-tab ${isActive ? 'active' : ''} px-4 py-2 text-sm font-medium rounded-t-lg border-b-2 ${activeClass} transition-colors"
                    data-section="${sectionId}" data-lang="${lang}">
                    ${langConfig?.name || lang}
                </button>
            `;
        }).join('');
    }

    /**
     * Generate HTML for download links
     */
    generateDownloadLinks(sectionId, languages) {
        return languages.map(lang => {
            const langConfig = LANGUAGE_CONFIG[lang];
            return `
                <a href="./examples/${lang}/${sectionId}.${langConfig.extension}" download 
                    class="text-primary-600 dark:text-primary-400 hover:text-primary-700 dark:hover:text-primary-300 text-sm font-medium">
                    <i class="fas fa-download mr-1"></i>Download ${langConfig.name}
                </a>
            `;
        }).join('\n<span class="text-secondary-400">•</span>\n');
    }

    /**
     * Create a section with dynamically loaded examples
     */
    async createSectionWithExamples(sectionId, sectionData) {
        const { title, description, languages = ['powershell', 'csharp', 'javascript', 'python'] } = sectionData;
        
        // Load all examples for this section
        const examples = {};
        for (const lang of languages) {
            examples[lang] = await this.loadExample(lang, sectionId);
        }

        return `
            <section id="${sectionId}" class="mb-16">
                <div class="bg-white dark:bg-secondary-800 rounded-lg shadow-md p-8">
                    <h3 class="text-2xl font-bold mb-6 dark:text-white">${title}</h3>
                    <p class="text-secondary-600 dark:text-secondary-300 mb-8">${description}</p>
                    
                    <!-- Language Example Card -->
                    <div class="bg-secondary-50 dark:bg-secondary-700 rounded-lg p-6">
                        <h4 class="text-lg font-semibold mb-4 dark:text-white"><i class="fas fa-laptop-code mr-2"></i>Code Examples</h4>
                        
                        <!-- Language Tabs -->
                        <div class="flex flex-wrap gap-2 mb-4 border-b border-secondary-200 dark:border-secondary-600">
                            ${this.generateLanguageTabs(sectionId, languages)}
                        </div>
                        
                        <!-- Code Samples -->
                        <div class="language-content">
                            ${this.generateLanguageContent(sectionId, examples, languages)}
                        </div>
                        
                        <!-- Download Links -->
                        <div class="mt-4 flex flex-wrap gap-4">
                            ${this.generateDownloadLinks(sectionId, languages)}
                        </div>
                    </div>
                </div>
            </section>
        `;
    }

    /**
     * Generate HTML for language content
     */
    generateLanguageContent(sectionId, examples, languages) {
        return languages.map((lang, index) => {
            const isActive = index === 0;
            const languageClass = this.getLanguageClass(lang);
            const codeId = `code-${sectionId}-${lang}`;
            
            return `
                <div id="${sectionId}-${lang}" class="language-sample ${isActive ? 'active' : 'hidden'}">
                    <div class="relative">
                        <pre class="code-block bg-secondary-900 text-secondary-100 p-4 rounded-lg overflow-x-auto text-sm"><code id="${codeId}" class="language-${languageClass}">${this.escapeHtml(examples[lang])}</code></pre>
                        <button onclick="copyCodeToClipboard('${codeId}')" 
                            class="copy-btn absolute top-2 right-2 bg-secondary-700 hover:bg-secondary-600 text-white px-3 py-1 rounded text-xs transition-colors">
                            <i class="fas fa-copy mr-1"></i>Copy
                        </button>
                    </div>
                </div>
            `;
        }).join('');
    }

    /**
     * Get the correct Prism.js language class
     */
    getLanguageClass(lang) {
        const mapping = {
            'powershell': 'powershell',
            'csharp': 'csharp',
            'javascript': 'javascript',
            'python': 'python',
            'java': 'java',
            'go': 'go'
        };
        return mapping[lang] || lang;
    }

    /**
     * Escape HTML entities
     */
    escapeHtml(text) {
        const div = document.createElement('div');
        div.textContent = text;
        return div.innerHTML;
    }

    /**
     * Escape text for JavaScript template literals
     */
    escapeForJs(text) {
        return text.replace(/`/g, '\\`').replace(/\$/g, '\\$').replace(/\\/g, '\\\\');
    }
}

// =============================================================================
// MAIN APPLICATION CLASS
// =============================================================================

class StorePosApp {
    constructor() {
        this.exampleLoader = new ExampleLoader();
        this.init();
    }

    init() {
        this.initializeEventListeners();
        this.initializeToastSystem();
        this.initializeTheme();
        this.initializeSearch();
        // Note: initializeScrollSpy will be called after content loads
    }

    // -------------------------------------------------------------------------
    // EVENT LISTENERS
    // -------------------------------------------------------------------------

    initializeEventListeners() {
        // Smooth scrolling for anchor links with proper offset
        document.querySelectorAll('a[href^="#"]').forEach(anchor => {
            anchor.addEventListener('click', (e) => {
                e.preventDefault();
                const targetId = anchor.getAttribute('href').substring(1);
                this.scrollToSection(targetId);
            });
        });

        // Initialize copy buttons
        this.initializeCopyButtons();
        
        // Add keyboard shortcuts
        this.initializeKeyboardShortcuts();
    }
    
    initializeKeyboardShortcuts() {
        document.addEventListener('keydown', (e) => {
            // Ctrl/Cmd + Shift + T for theme toggle
            if ((e.ctrlKey || e.metaKey) && e.shiftKey && e.key === 'T') {
                e.preventDefault();
                this.toggleTheme();
            }
            
            // Escape to close search results
            if (e.key === 'Escape') {
                const searchResults = document.getElementById('search-results');
                if (searchResults) {
                    searchResults.innerHTML = '';
                    searchResults.style.display = 'none';
                }
            }
        });
    }

    initializeCopyButtons() {
        // Use event delegation to handle copy buttons
        document.addEventListener('click', (e) => {
            if (e.target.closest('.copy-btn')) {
                e.preventDefault();
                const button = e.target.closest('.copy-btn');
                const codeElement = button.parentElement.querySelector('code');
                if (codeElement) {
                    this.copyToClipboard(codeElement.textContent, button);
                }
            }
        });
    }

    // New method to copy code by element ID
    copyCodeFromElement(elementId) {
        const codeElement = document.getElementById(elementId);
        if (codeElement) {
            this.copyToClipboard(codeElement.textContent);
        }
    }

    // -------------------------------------------------------------------------
    // TOAST SYSTEM
    // -------------------------------------------------------------------------

    initializeToastSystem() {
        // Create toast container if it doesn't exist
        if (!document.getElementById('toast-container')) {
            const toastContainer = document.createElement('div');
            toastContainer.id = 'toast-container';
            toastContainer.className = 'fixed top-4 right-4 z-50 space-y-2';
            document.body.appendChild(toastContainer);
        }
    }

    showToast(message, type = 'info') {
        const toast = document.createElement('div');
        const toastId = 'toast-' + Date.now();
        
        const bgColor = {
            success: 'bg-green-500',
            error: 'bg-red-500',
            warning: 'bg-yellow-500',
            info: 'bg-blue-500'
        }[type] || 'bg-blue-500';

        toast.id = toastId;
        toast.className = `toast ${bgColor} text-white px-4 py-3 rounded-lg shadow-lg transform translate-x-full transition-transform duration-300`;
        toast.innerHTML = `
            <div class="flex items-center justify-between">
                <span>${message}</span>
                <button onclick="app.hideToast('${toastId}')" class="ml-4 text-white hover:text-gray-200">
                    ×
                </button>
            </div>
        `;

        const container = document.getElementById('toast-container');
        container.appendChild(toast);

        // Show toast
        setTimeout(() => {
            toast.classList.remove('translate-x-full');
        }, 100);

        // Auto-hide after 3 seconds
        setTimeout(() => {
            this.hideToast(toastId);
        }, 3000);
    }

    hideToast(toastId) {
        const toast = document.getElementById(toastId);
        if (toast) {
            toast.classList.add('translate-x-full');
            setTimeout(() => {
                toast.remove();
            }, 300);
        }
    }

    // -------------------------------------------------------------------------
    // CLIPBOARD FUNCTIONALITY
    // -------------------------------------------------------------------------

    async copyToClipboard(text, button = null) {
        try {
            await navigator.clipboard.writeText(text);
            this.showToast('Code copied to clipboard!', 'success');
            
            if (button) {
                // Temporarily change button content
                const originalContent = button.innerHTML;
                button.innerHTML = '<i class="fas fa-check mr-1"></i>Copied!';
                button.classList.add('bg-accent-600');
                button.classList.remove('bg-secondary-700');
                
                setTimeout(() => {
                    button.innerHTML = originalContent;
                    button.classList.remove('bg-accent-600');
                    button.classList.add('bg-secondary-700');
                }, 2000);
            }
        } catch (err) {
            console.error('Failed to copy text: ', err);
            this.showToast('Failed to copy code', 'error');
        }
    }

    // -------------------------------------------------------------------------
    // SEARCH FUNCTIONALITY
    // -------------------------------------------------------------------------

    initializeSearch() {
        const searchInput = document.getElementById('guide-search');
        if (searchInput) {
            searchInput.addEventListener('input', (e) => {
                this.searchGuide(e.target.value);
            });
            
            // Add click outside to close search results
            document.addEventListener('click', (e) => {
                const searchContainer = document.querySelector('.search-container');
                const resultsContainer = document.getElementById('search-results');
                
                if (resultsContainer && !searchContainer.contains(e.target)) {
                    resultsContainer.style.display = 'none';
                }
            });
            
            // Show search results when clicking on input
            searchInput.addEventListener('focus', () => {
                if (searchInput.value.length >= 2) {
                    this.searchGuide(searchInput.value);
                }
            });
        }
    }

    searchGuide(query) {
        const resultsContainer = this.getOrCreateSearchResults();
        
        // Clear results if query is empty or too short
        if (!query || query.length < 2) {
            resultsContainer.innerHTML = '';
            resultsContainer.style.display = 'none';
            return;
        }

        const sections = document.querySelectorAll('section[id]');
        const searchResults = [];

        sections.forEach(section => {
            const content = section.textContent.toLowerCase();
            const title = section.querySelector('h3')?.textContent || section.querySelector('h2')?.textContent || 'Section';
            
            if (content.includes(query.toLowerCase())) {
                // Find the context around the match
                const index = content.indexOf(query.toLowerCase());
                const start = Math.max(0, index - 50);
                const end = Math.min(content.length, index + query.length + 100);
                const snippet = content.substring(start, end);
                
                searchResults.push({
                    id: section.id,
                    title: title,
                    content: (start > 0 ? '...' : '') + snippet + (end < content.length ? '...' : '')
                });
            }
        });

        this.displaySearchResults(searchResults);
    }

    getOrCreateSearchResults() {
        let resultsContainer = document.getElementById('search-results');
        if (!resultsContainer) {
            resultsContainer = document.createElement('div');
            resultsContainer.id = 'search-results';
            resultsContainer.className = 'absolute top-full left-0 w-full bg-white dark:bg-secondary-800 border border-secondary-300 dark:border-secondary-600 rounded-lg shadow-lg mt-1 z-50 max-h-64 overflow-y-auto';
            document.querySelector('.search-container').appendChild(resultsContainer);
        }
        return resultsContainer;
    }

    displaySearchResults(results) {
        const resultsContainer = this.getOrCreateSearchResults();

        if (results.length === 0) {
            resultsContainer.innerHTML = '<div class="p-4 text-secondary-500 dark:text-secondary-400">No results found</div>';
            resultsContainer.style.display = 'block';
            return;
        }

        resultsContainer.innerHTML = results.map((result, index) => `
            <div class="p-3 hover:bg-secondary-50 dark:hover:bg-secondary-700 ${index < results.length - 1 ? 'border-b border-secondary-200 dark:border-secondary-600' : ''} cursor-pointer transition-colors" onclick="app.scrollToSection('${result.id}')">
                <div class="font-medium text-secondary-900 dark:text-white">${result.title}</div>
                <div class="text-sm text-secondary-600 dark:text-secondary-300">${result.content}</div>
            </div>
        `).join('');
        resultsContainer.style.display = 'block';
    }

    scrollToSection(sectionId) {
        const section = document.getElementById(sectionId);
        if (section) {
            // Calculate offset for sticky navigation (nav height + some padding)
            const navHeight = document.querySelector('nav').offsetHeight || 80;
            const headerHeight = document.querySelector('header').offsetHeight || 90;
            const offset = navHeight + headerHeight + 20; // Extra padding
            
            const elementPosition = section.getBoundingClientRect().top;
            const offsetPosition = elementPosition + window.pageYOffset - offset;

            window.scrollTo({
                top: offsetPosition,
                behavior: 'smooth'
            });

            // Hide search results
            const resultsContainer = document.getElementById('search-results');
            if (resultsContainer) {
                resultsContainer.style.display = 'none';
            }
            // Clear search input
            const searchInput = document.getElementById('guide-search');
            if (searchInput) {
                searchInput.value = '';
            }
        }
    }

    // -------------------------------------------------------------------------
    // SCROLL SPY FUNCTIONALITY
    // -------------------------------------------------------------------------

    initializeScrollSpy() {
        // Throttle scroll events for better performance
        let ticking = false;
        
        const updateActiveSection = () => {
            // Order sections as they appear in the DOM
            const sections = ['overview', 'authentication', 'products', 'sales', 'users', 'examples'];
            const navHeight = document.querySelector('nav').offsetHeight || 80;
            const headerHeight = document.querySelector('header').offsetHeight || 90;
            const offset = navHeight + headerHeight + 50; // Reduced buffer for better detection
            
            let activeSection = null;
            
            // Special case: if we're at the very top of the page, highlight the first section
            if (window.pageYOffset < 200) {
                activeSection = sections[0]; // 'overview'
            } else {
                // Find the section that's currently in view
                for (const sectionId of sections) {
                    const section = document.getElementById(sectionId);
                    if (section) {
                        const rect = section.getBoundingClientRect();
                        // Check if section is in viewport with proper offset
                        if (rect.top <= offset && rect.bottom > offset) {
                            activeSection = sectionId;
                            break;
                        }
                    }
                }
                
                // If no section is exactly in the offset range, find the one closest to the top
                if (!activeSection) {
                    let topSection = null;
                    let smallestPositiveTop = Infinity;
                    
                    for (const sectionId of sections) {
                        const section = document.getElementById(sectionId);
                        if (section) {
                            const rect = section.getBoundingClientRect();
                            // Find the section closest to the top that's above the fold
                            if (rect.top < offset && rect.top > -rect.height) {
                                if (rect.top > smallestPositiveTop - rect.height) {
                                    topSection = sectionId;
                                    smallestPositiveTop = rect.top;
                                }
                            }
                        }
                    }
                    
                    if (topSection) {
                        activeSection = topSection;
                    } else {
                        // Fallback: find the section with the smallest distance to the offset
                        let closestSection = null;
                        let closestDistance = Infinity;
                        
                        for (const sectionId of sections) {
                            const section = document.getElementById(sectionId);
                            if (section) {
                                const rect = section.getBoundingClientRect();
                                const distance = Math.abs(rect.top - offset);
                                if (distance < closestDistance) {
                                    closestDistance = distance;
                                    closestSection = sectionId;
                                }
                            }
                        }
                        activeSection = closestSection;
                    }
                }
            }
            
            // Update navigation active states
            if (activeSection) {
                this.updateActiveNavItem(activeSection);
            }
            
            ticking = false;
        };
        
        const requestTick = () => {
            if (!ticking) {
                requestAnimationFrame(updateActiveSection);
                ticking = true;
            }
        };
        
        // Listen for scroll events
        window.addEventListener('scroll', requestTick);
        
        // Initial check after a short delay to ensure DOM is ready
        setTimeout(() => {
            updateActiveSection();
        }, 100);
    }

    updateActiveNavItem(activeSection) {
        // Remove active class from all nav items
        const navLinks = document.querySelectorAll('nav a[href^="#"]');
        navLinks.forEach(link => {
            link.classList.remove('border-primary-500', 'text-primary-600', 'dark:text-primary-400');
            link.classList.add('border-transparent', 'text-secondary-500', 'hover:text-secondary-700', 'dark:text-secondary-400', 'dark:hover:text-secondary-200');
        });
        
        // Add active class to current section nav item
        const activeLink = document.querySelector(`nav a[href="#${activeSection}"]`);
        if (activeLink) {
            activeLink.classList.add('border-primary-500', 'text-primary-600', 'dark:text-primary-400');
            activeLink.classList.remove('border-transparent', 'text-secondary-500', 'hover:text-secondary-700', 'dark:text-secondary-400', 'dark:hover:text-secondary-200');
        }
    }

    // -------------------------------------------------------------------------
    // THEME FUNCTIONALITY
    // -------------------------------------------------------------------------

    toggleTheme() {
        const html = document.documentElement;
        const body = document.body;
        const isDark = html.classList.contains('dark');
        
        if (isDark) {
            html.classList.remove('dark');
            body.classList.remove('dark');
            localStorage.setItem('theme', 'light');
            this.showToast('Switched to light theme', 'success');
        } else {
            html.classList.add('dark');
            body.classList.add('dark');
            localStorage.setItem('theme', 'dark');
            this.showToast('Switched to dark theme', 'success');
        }
        
        this.updateThemeToggleIcon();
        
        // Force repaint to ensure all elements update
        document.body.style.display = 'none';
        document.body.offsetHeight; // Trigger reflow
        document.body.style.display = '';
    }

    updateThemeToggleIcon() {
        const themeButton = document.querySelector('[onclick*="toggleTheme"]');
        const themeTooltip = document.getElementById('theme-tooltip');
        if (!themeButton) return;
        
        const isDark = document.documentElement.classList.contains('dark');
        const svg = themeButton.querySelector('svg');
        
        if (isDark) {
            // Sun icon for light mode toggle
            svg.innerHTML = `<path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 3v1m0 16v1m9-9h-1M4 12H3m15.364 6.364l-.707-.707M6.343 6.343l-.707-.707m12.728 0l-.707.707M6.343 17.657l-.707.707M16 12a4 4 0 11-8 0 4 4 0 018 0z"></path>`;
            if (themeTooltip) themeTooltip.textContent = 'Switch to light theme';
        } else {
            // Moon icon for dark mode toggle
            svg.innerHTML = `<path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M20.354 15.354A9 9 0 018.646 3.646 9.003 9.003 0 0012 21a9.003 9.003 0 008.354-5.646z"></path>`;
            if (themeTooltip) themeTooltip.textContent = 'Switch to dark theme';
        }
    }

    initializeTheme() {
        const savedTheme = localStorage.getItem('theme');
        const systemPrefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches;
        
        if (savedTheme === 'dark' || (!savedTheme && systemPrefersDark)) {
            document.documentElement.classList.add('dark');
            document.body.classList.add('dark');
        }
        
        this.updateThemeToggleIcon();
        
        // Listen for system theme changes
        window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', (e) => {
            if (!localStorage.getItem('theme')) {
                if (e.matches) {
                    document.documentElement.classList.add('dark');
                    document.body.classList.add('dark');
                } else {
                    document.documentElement.classList.remove('dark');
                    document.body.classList.remove('dark');
                }
                this.updateThemeToggleIcon();
            }
        });
    }

    // -------------------------------------------------------------------------
    // API TESTING
    // -------------------------------------------------------------------------

    async testApiEndpoint(endpoint, method = 'GET', data = null, token = null) {
        const baseUrl = 'http://localhost:5062';
        const url = `${baseUrl}${endpoint}`;
        
        const options = {
            method: method,
            headers: {
                'Content-Type': 'application/json'
            }
        };

        if (token) {
            options.headers['Authorization'] = `Bearer ${token}`;
        }

        if (data && ['POST', 'PUT', 'PATCH'].includes(method)) {
            options.body = JSON.stringify(data);
        }

        try {
            const response = await fetch(url, options);
            const result = await response.json();
            
            return {
                success: response.ok,
                status: response.status,
                data: result
            };
        } catch (error) {
            return {
                success: false,
                error: error.message
            };
        }
    }

    async testApiConnection() {
        const urlInput = document.getElementById('api-test-url');
        const resultDiv = document.getElementById('api-test-result');
        
        if (!urlInput || !resultDiv) {
            console.error('API test elements not found');
            return;
        }

        const url = urlInput.value;
        
        resultDiv.className = 'mt-4 p-4 rounded-lg bg-secondary-100 dark:bg-secondary-700 border border-secondary-200 dark:border-secondary-600';
                        resultDiv.innerHTML = '<div class="text-secondary-600 dark:text-secondary-300"><i class="fas fa-spinner fa-spin mr-2"></i>Testing connection...</div>';
        resultDiv.classList.remove('hidden');
        
        try {
            const response = await fetch(`${url}/swagger/index.html`);
            if (response.ok) {
                resultDiv.className = 'mt-4 p-4 rounded-lg bg-accent-50 dark:bg-accent-900/20 border border-accent-200 dark:border-accent-700';
                resultDiv.innerHTML = `
                    <div class="text-accent-800 dark:text-accent-200">
                        <i class="fas fa-check-circle mr-2"></i><strong>Connection successful!</strong><br>
                        <small class="text-accent-600 dark:text-accent-400">API is running and accessible at ${url}</small>
                    </div>
                `;
                this.showToast('API connection successful!', 'success');
            } else {
                throw new Error(`HTTP ${response.status}`);
            }
        } catch (error) {
            resultDiv.className = 'mt-4 p-4 rounded-lg bg-red-50 dark:bg-red-900/20 border border-red-200 dark:border-red-700';
            resultDiv.innerHTML = `
                <div class="text-red-800 dark:text-red-200">
                    <i class="fas fa-times-circle mr-2"></i><strong>Connection failed!</strong><br>
                    <small class="text-red-600 dark:text-red-400">Error: ${error.message}</small><br>
                    <small class="text-red-600 dark:text-red-400">Make sure the API server is running at the specified URL.</small>
                </div>
            `;
            this.showToast('API connection failed!', 'error');
        }
    }

    // -------------------------------------------------------------------------
    // CONTENT LOADING
    // -------------------------------------------------------------------------

    async loadContent() {
        const contentSections = document.getElementById('content-sections');
        if (!contentSections) return;

        this.populateLanguageCards();

        const sectionsToLoad = Object.keys(SECTIONS_CONFIG);
        console.log('Loading documentation sections...');

        for (const sectionId of sectionsToLoad) {
            try {
                const sectionConfig = SECTIONS_CONFIG[sectionId];

                // Try to load from HTML file first
                const response = await fetch(`./sections/${sectionId}.html`);
                if (response.ok) {
                    const html = await response.text();
                    contentSections.innerHTML += html;
                    console.log(`✅ Loaded section from file: ${sectionId}`);
                } else {
                    // Fallback: generate section with dynamic examples
                    console.log(`📁 Generating dynamic section: ${sectionId}`);
                    const sectionHtml = await this.exampleLoader.createSectionWithExamples(sectionId, sectionConfig);
                    contentSections.innerHTML += sectionHtml;
                }
            } catch (error) {
                console.error(`❌ Error loading section ${sectionId}:`, error);
                this.createFallbackSection(contentSections, sectionId);
            }
        }

        // Initialize syntax highlighting
        if (typeof Prism !== 'undefined') {
            Prism.highlightAll();
            console.log('✅ Syntax highlighting initialized');
        }

        // Initialize scroll spy after content is loaded
        this.initializeScrollSpy();
        console.log('✅ Scroll spy initialized');

        console.log('✅ Documentation loaded successfully');
    }

    createFallbackSection(container, sectionId) {
        const sectionConfig = SECTIONS_CONFIG[sectionId] || {
            title: sectionId.charAt(0).toUpperCase() + sectionId.slice(1),
            description: `Documentation for ${sectionId} functionality.`
        };

        container.innerHTML += `
            <section id="${sectionId}" class="mb-16">
                <div class="bg-white dark:bg-secondary-800 rounded-lg shadow-md p-8">
                    <h3 class="text-2xl font-bold mb-6 dark:text-white">${sectionConfig.title}</h3>
                    <p class="text-secondary-600 dark:text-secondary-300 mb-8">${sectionConfig.description}</p>
                    <div class="bg-yellow-50 dark:bg-yellow-900/20 border border-yellow-200 dark:border-yellow-800 rounded-lg p-4">
                        <p class="text-yellow-800 dark:text-yellow-200"><i class="fas fa-exclamation-triangle mr-2"></i>Examples are loading... Please check that example files are available.</p>
                    </div>
                </div>
            </section>
        `;
    }

    populateLanguageCards() {
        const languageCards = document.getElementById('language-cards');
        if (!languageCards) return;

        const languages = ['powershell', 'csharp', 'javascript', 'python', 'java', 'go'];

        languageCards.innerHTML = languages.map(langId => {
            const lang = LANGUAGE_CONFIG[langId];
            return `
                <div class="p-6 border border-secondary-200 dark:border-secondary-700 rounded-lg hover:shadow-lg transition-shadow">
                    <div class="flex items-center mb-4">
                        <div class="w-10 h-10 ${lang.bgClass} rounded-lg flex items-center justify-center mr-3">
                            <span class="${lang.textClass} font-bold text-sm">${lang.icon}</span>
                        </div>
                        <h4 class="text-lg font-semibold dark:text-white">${lang.name}</h4>
                    </div>
                    <p class="text-secondary-600 dark:text-secondary-300 text-sm mb-4">
                        ${lang.description}
                    </p>
                    <div class="flex flex-wrap gap-2">
                        <span class="text-secondary-500 dark:text-secondary-400 text-sm">Coming Soon</span>
                    </div>
                </div>
            `;
        }).join('');
    }
}

// =============================================================================
// GLOBAL FUNCTIONS
// =============================================================================

/**
 * Global function for language tab switching
 */
function showLanguageTab(sectionId, language) {
    // Hide all language samples for this section
    const samples = document.querySelectorAll(`[id^="${sectionId}-"]`);
    samples.forEach(sample => {
        sample.classList.add('hidden');
        sample.classList.remove('active');
    });

    // Show the selected language sample
    const targetSample = document.getElementById(`${sectionId}-${language}`);
    if (targetSample) {
        targetSample.classList.remove('hidden');
        targetSample.classList.add('active');
    }

    // Update tab styles
    const tabs = document.querySelectorAll(`.language-tab[data-section="${sectionId}"]`);
    tabs.forEach(tab => {
        tab.classList.remove('active', 'border-primary-500', 'text-primary-600', 'dark:text-primary-400');
        tab.classList.add('border-transparent', 'text-secondary-500', 'hover:text-secondary-700', 'dark:text-secondary-400', 'dark:hover:text-secondary-200');
    });

    // Activate the selected tab
    const activeTab = document.querySelector(`.language-tab[data-section="${sectionId}"][data-lang="${language}"]`);
    if (activeTab) {
        activeTab.classList.add('active', 'border-primary-500', 'text-primary-600', 'dark:text-primary-400');
        activeTab.classList.remove('border-transparent', 'text-secondary-500', 'hover:text-secondary-700', 'dark:text-secondary-400', 'dark:hover:text-secondary-200');
    }

    // Re-highlight syntax
    if (typeof Prism !== 'undefined') {
        Prism.highlightAll();
    }
}

/**
 * Global function for copying to clipboard
 */
function copyToClipboard(text) {
    if (window.app) {
        window.app.copyToClipboard(text);
    }
}

/**
 * Global function for theme toggle
 */
function toggleTheme() {
    if (window.app) {
        window.app.toggleTheme();
    }
}

/**
 * Global function for API connection testing
 */
function testApiConnection() {
    if (window.app) {
        window.app.testApiConnection();
    }
}

/**
 * Global function for copying code by element ID
 */
function copyCodeToClipboard(elementId) {
    if (window.app) {
        window.app.copyCodeFromElement(elementId);
    }
}

// =============================================================================
// INITIALIZATION
// =============================================================================

// Initialize the application when DOM is loaded
document.addEventListener('DOMContentLoaded', async () => {
    window.app = new StorePosApp();
    
    // Expose configurations globally for backward compatibility
    window.sectionsConfig = SECTIONS_CONFIG;
    window.ExampleLoader = ExampleLoader;
    
    // Load content
    await window.app.loadContent();
});
