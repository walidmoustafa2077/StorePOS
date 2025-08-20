class StorePosApiClient {
    constructor(baseUrl = 'http://localhost:5062') {
        this.baseUrl = baseUrl;
        this.accessToken = null;
    }
    
    async login(usernameOrEmail, password) {
        try {
            const response = await fetch(`${this.baseUrl}/api/auth/login`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    usernameOrEmail,
                    password
                })
            });
            
            const result = await response.json();
            
            if (result.success) {
                this.accessToken = result.data.accessToken;
                console.log('Login successful!');
                return true;
            } else {
                console.error('Login failed:', result.message);
                return false;
            }
        } catch (error) {
            console.error('Login error:', error);
            return false;
        }
    }
    
    async getProducts() {
        if (!this.accessToken) {
            throw new Error('Not authenticated. Please login first.');
        }
        
        try {
            const response = await fetch(`${this.baseUrl}/api/products`, {
                headers: {
                    'Authorization': `Bearer ${this.accessToken}`,
                    'Content-Type': 'application/json'
                }
            });
            
            if (response.ok) {
                return await response.json();
            } else {
                throw new Error(`HTTP ${response.status}: ${response.statusText}`);
            }
        } catch (error) {
            console.error('Error fetching products:', error);
            throw error;
        }
    }
    
    async createProduct(productData) {
        if (!this.accessToken) {
            throw new Error('Not authenticated. Please login first.');
        }
        
        try {
            const response = await fetch(`${this.baseUrl}/api/products`, {
                method: 'POST',
                headers: {
                    'Authorization': `Bearer ${this.accessToken}`,
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(productData)
            });
            
            if (response.ok) {
                return await response.json();
            } else {
                throw new Error(`HTTP ${response.status}: ${response.statusText}`);
            }
        } catch (error) {
            console.error('Error creating product:', error);
            throw error;
        }
    }
    
    async updateProduct(id, productData) {
        if (!this.accessToken) {
            throw new Error('Not authenticated. Please login first.');
        }
        
        try {
            const response = await fetch(`${this.baseUrl}/api/products/${id}`, {
                method: 'PUT',
                headers: {
                    'Authorization': `Bearer ${this.accessToken}`,
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(productData)
            });
            
            if (response.ok) {
                return await response.json();
            } else {
                throw new Error(`HTTP ${response.status}: ${response.statusText}`);
            }
        } catch (error) {
            console.error('Error updating product:', error);
            throw error;
        }
    }
    
    async deleteProduct(id) {
        if (!this.accessToken) {
            throw new Error('Not authenticated. Please login first.');
        }
        
        try {
            const response = await fetch(`${this.baseUrl}/api/products/${id}`, {
                method: 'DELETE',
                headers: {
                    'Authorization': `Bearer ${this.accessToken}`,
                    'Content-Type': 'application/json'
                }
            });
            
            return response.ok;
        } catch (error) {
            console.error('Error deleting product:', error);
            return false;
        }
    }
}

// Usage example for Node.js
async function example() {
    const client = new StorePosApiClient();
    
    if (await client.login('admin@storepos.com', 'Admin123!')) {
        try {
            // Get all products
            const products = await client.getProducts();
            console.log(`Retrieved ${products.length} products`);
            
            // Create a new product
            const newProduct = {
                sku: 'JS-001',
                barcode: '1234567890123',
                name: 'Product from JavaScript',
                category: 'Test',
                price: 25.99,
                cost: 15.00,
                stockQty: 100,
                isActive: true
            };
            
            const created = await client.createProduct(newProduct);
            console.log(`Created product with ID: ${created.id}`);
            
            // Update the product
            const updated = await client.updateProduct(created.id, {
                ...newProduct,
                name: 'Updated Product from JavaScript',
                price: 29.99
            });
            console.log(`Updated product: ${updated.name}`);
            
            // Delete the product
            const deleted = await client.deleteProduct(created.id);
            console.log(`Product deleted: ${deleted}`);
            
        } catch (error) {
            console.error('Failed to manage products:', error);
        }
    }
}

// Export for Node.js
if (typeof module !== 'undefined' && module.exports) {
    module.exports = StorePosApiClient;
}

// Run example if this is the main file
if (typeof require !== 'undefined' && require.main === module) {
    example();
}
