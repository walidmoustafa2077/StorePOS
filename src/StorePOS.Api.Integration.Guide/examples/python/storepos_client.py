import requests
import json
from typing import Optional, List, Dict

class StorePosApiClient:
    def __init__(self, base_url: str = "http://localhost:5062"):
        self.base_url = base_url
        self.access_token: Optional[str] = None
        self.session = requests.Session()
    
    def login(self, username_or_email: str, password: str) -> bool:
        """Login and store access token"""
        login_data = {
            "usernameOrEmail": username_or_email,
            "password": password
        }
        
        try:
            response = self.session.post(
                f"{self.base_url}/api/auth/login",
                json=login_data,
                headers={"Content-Type": "application/json"}
            )
            
            if response.status_code == 200:
                result = response.json()
                if result.get("success"):
                    self.access_token = result["data"]["accessToken"]
                    # Set default authorization header
                    self.session.headers.update({
                        "Authorization": f"Bearer {self.access_token}"
                    })
                    print("Login successful!")
                    return True
                else:
                    print(f"Login failed: {result.get('message')}")
                    return False
            else:
                print(f"HTTP Error {response.status_code}: {response.text}")
                return False
                
        except requests.exceptions.RequestException as e:
            print(f"Request error: {e}")
            return False
    
    def get_products(self) -> Optional[List[Dict]]:
        """Get all products"""
        if not self.access_token:
            raise Exception("Not authenticated. Please login first.")
        
        try:
            response = self.session.get(f"{self.base_url}/api/products")
            
            if response.status_code == 200:
                return response.json()
            else:
                print(f"Error getting products: {response.status_code}")
                return None
                
        except requests.exceptions.RequestException as e:
            print(f"Request error: {e}")
            return None
    
    def create_product(self, product_data: Dict) -> Optional[Dict]:
        """Create a new product"""
        if not self.access_token:
            raise Exception("Not authenticated. Please login first.")
        
        try:
            response = self.session.post(
                f"{self.base_url}/api/products",
                json=product_data
            )
            
            if response.status_code == 201:
                return response.json()
            else:
                print(f"Error creating product: {response.status_code}")
                return None
                
        except requests.exceptions.RequestException as e:
            print(f"Request error: {e}")
            return None

# Usage example
if __name__ == "__main__":
    client = StorePosApiClient()
    
    if client.login("admin@storepos.com", "Admin123!"):
        products = client.get_products()
        if products:
            print(f"Retrieved {len(products)} products")
            
            # Create a new product example
            new_product = {
                "sku": "PYTHON-001",
                "barcode": "1234567890123",
                "name": "Product from Python",
                "category": "Test",
                "price": 19.99,
                "cost": 12.00,
                "stockQty": 50,
                "isActive": True
            }
            
            created = client.create_product(new_product)
            if created:
                print(f"Created product with ID: {created['id']}")
        else:
            print("Failed to retrieve products")
