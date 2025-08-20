// StorePOS API Integration Guide JavaScript

class StorePosGuide {
  constructor() {
    this.initializeEventListeners();
    this.initializeToastSystem();
  }

  initializeEventListeners() {
    // Smooth scrolling for anchor links
    document.querySelectorAll('a[href^="#"]').forEach(anchor => {
      anchor.addEventListener('click', (e) => {
        e.preventDefault();
        const target = document.querySelector(anchor.getAttribute('href'));
        if (target) {
          target.scrollIntoView({
            behavior: 'smooth',
            block: 'start'
          });
        }
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
        }
      }
    });
  }

  initializeCopyButtons() {
    document.querySelectorAll('.copy-button').forEach(button => {
      button.addEventListener('click', () => {
        const codeId = button.getAttribute('data-code-id');
        const codeElement = document.getElementById(codeId);
        if (codeElement) {
          this.copyToClipboard(codeElement.textContent, button);
        }
      });
    });
  }

  initializeToastSystem() {
    // Create toast container if it doesn't exist
    if (!document.getElementById('toast-container')) {
      const toastContainer = document.createElement('div');
      toastContainer.id = 'toast-container';
      toastContainer.className = 'fixed top-4 right-4 z-50 space-y-2';
      document.body.appendChild(toastContainer);
    }
  }

  // Tab switching functionality
  showTab(tabName, element) {
    // Hide all tab contents
    document.querySelectorAll('.tab-content').forEach(tab => {
      tab.classList.remove('active');
    });

    // Remove active class from all tab buttons in the same group
    const tabGroup = element.closest('.tab-group') || document;
    tabGroup.querySelectorAll('.tab-button').forEach(btn => {
      btn.classList.remove('active', 'bg-blue-600', 'text-white');
      btn.classList.add('bg-gray-200', 'text-gray-700');
    });

    // Show selected tab content
    const targetTab = document.getElementById(tabName);
    if (targetTab) {
      targetTab.classList.add('active');
    }

    // Add active class to clicked button
    element.classList.add('active', 'bg-blue-600', 'text-white');
    element.classList.remove('bg-gray-200', 'text-gray-700');
  }

  // Enhanced copy to clipboard functionality
  async copyToClipboard(text, button = null) {
    try {
      await navigator.clipboard.writeText(text);
      this.showToast('Code copied to clipboard!', 'success');
      
      if (button) {
        // Temporarily change button text
        const originalText = button.innerHTML;
        button.innerHTML = '✅ Copied!';
        button.classList.add('bg-green-600');
        
        setTimeout(() => {
          button.innerHTML = originalText;
          button.classList.remove('bg-green-600');
        }, 2000);
      }
    } catch (err) {
      console.error('Failed to copy text: ', err);
      this.showToast('Failed to copy code', 'error');
    }
  }

  // Toast notification system
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
        <button onclick="guide.hideToast('${toastId}')" class="ml-4 text-white hover:text-gray-200">
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

  // API Testing functionality
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

  // Search functionality for the guide
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
    const resultsContainer = document.getElementById('search-results');
    
    // Clear results if query is empty or too short
    if (!query || query.length < 2) {
      if (resultsContainer) {
        resultsContainer.innerHTML = '';
        resultsContainer.style.display = 'none';
      }
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

  displaySearchResults(results) {
    let resultsContainer = document.getElementById('search-results');
    if (!resultsContainer) {
      resultsContainer = document.createElement('div');
      resultsContainer.id = 'search-results';
      resultsContainer.className = 'absolute top-full left-0 w-full bg-white dark:bg-gray-800 border border-gray-300 dark:border-gray-600 rounded-lg shadow-lg mt-1 z-50 max-h-64 overflow-y-auto';
      document.querySelector('.search-container').appendChild(resultsContainer);
    }

    if (results.length === 0) {
      resultsContainer.innerHTML = '<div class="p-4 text-gray-500 dark:text-gray-400">No results found</div>';
      resultsContainer.style.display = 'block';
      return;
    }

    resultsContainer.innerHTML = results.map((result, index) => `
      <div class="p-3 hover:bg-gray-50 dark:hover:bg-gray-700 ${index < results.length - 1 ? 'border-b border-gray-200 dark:border-gray-600' : ''} cursor-pointer transition-colors" onclick="guide.scrollToSection('${result.id}')">
        <div class="font-medium text-gray-900 dark:text-white">${result.title}</div>
        <div class="text-sm text-gray-600 dark:text-gray-300">${result.content}</div>
      </div>
    `).join('');
    resultsContainer.style.display = 'block';
  }

  scrollToSection(sectionId) {
    const section = document.getElementById(sectionId);
    if (section) {
      section.scrollIntoView({ behavior: 'smooth', block: 'start' });
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

  // Theme switching functionality
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
    
    // Update theme toggle button icon
    this.updateThemeToggleIcon();
    
    // Force repaint to ensure all elements update
    document.body.style.display = 'none';
    document.body.offsetHeight; // Trigger reflow
    document.body.style.display = '';
  }

  // Update theme toggle button icon
  updateThemeToggleIcon() {
    const themeButton = document.querySelector('[onclick="guide.toggleTheme()"]');
    if (!themeButton) return;
    
    const isDark = document.documentElement.classList.contains('dark');
    const svg = themeButton.querySelector('svg');
    
    if (isDark) {
      // Sun icon for light mode toggle
      svg.innerHTML = `<path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 3v1m0 16v1m9-9h-1M4 12H3m15.364 6.364l-.707-.707M6.343 6.343l-.707-.707m12.728 0l-.707.707M6.343 17.657l-.707.707M16 12a4 4 0 11-8 0 4 4 0 018 0z"></path>`;
      themeButton.title = 'Switch to light theme (Ctrl+Shift+T)';
    } else {
      // Moon icon for dark mode toggle
      svg.innerHTML = `<path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M20.354 15.354A9 9 0 018.646 3.646 9.003 9.003 0 0012 21a9.003 9.003 0 008.354-5.646z"></path>`;
      themeButton.title = 'Switch to dark theme (Ctrl+Shift+T)';
    }
  }

  // Initialize theme from localStorage or system preference
  initializeTheme() {
    const savedTheme = localStorage.getItem('theme');
    const systemPrefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches;
    
    if (savedTheme === 'dark' || (!savedTheme && systemPrefersDark)) {
      document.documentElement.classList.add('dark');
      document.body.classList.add('dark');
    }
    
    // Update the theme toggle icon on initialization
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
}

// Global functions for inline event handlers (for backward compatibility)
function showTab(tabName, element) {
  guide.showTab(tabName, element);
}

function copyToClipboard(text) {
  guide.copyToClipboard(text);
}

// Language tab switching function
function showLanguageTab(sectionId, language) {
  // Hide all language samples for this section
  const allSamples = document.querySelectorAll(`#${sectionId} .language-sample`);
  allSamples.forEach(sample => {
    sample.classList.add('hidden');
    sample.classList.remove('active');
  });
  
  // Show the selected language sample
  const targetSample = document.getElementById(`${sectionId}-${language}`);
  if (targetSample) {
    targetSample.classList.remove('hidden');
    targetSample.classList.add('active');
  }
  
  // Update tab buttons for this section
  const allTabs = document.querySelectorAll(`[data-section="${sectionId}"].language-tab`);
  allTabs.forEach(tab => {
    tab.classList.remove('active', 'border-blue-500', 'text-blue-600', 'dark:text-blue-400');
    tab.classList.add('border-transparent', 'text-gray-500', 'hover:text-gray-700', 'dark:text-gray-400', 'dark:hover:text-gray-200');
  });
  
  // Activate the clicked tab
  const activeTab = document.querySelector(`[data-section="${sectionId}"][data-lang="${language}"]`);
  if (activeTab) {
    activeTab.classList.add('active', 'border-blue-500', 'text-blue-600', 'dark:text-blue-400');
    activeTab.classList.remove('border-transparent', 'text-gray-500', 'hover:text-gray-700', 'dark:text-gray-400', 'dark:hover:text-gray-200');
  }
  
  // Re-highlight syntax if Prism is available
  if (typeof Prism !== 'undefined') {
    setTimeout(() => {
      Prism.highlightAll();
    }, 100);
  }
}

// Initialize the guide when DOM is loaded
let guide;
document.addEventListener('DOMContentLoaded', () => {
  guide = new StorePosGuide();
  guide.initializeTheme();
  guide.initializeSearch();
});
