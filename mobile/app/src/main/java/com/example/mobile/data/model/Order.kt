package com.example.mobile.data.model

/*
* Order request body:
* {
*   "orderId": "string",
*   "status": 0,
*   "orderDate": "2024-10-01T11:01:39.560Z",
*   "orderItems": [
*     {
*       "orderItemId": "string",
*       "productId": "string",
*       "productName": "string",
*       "quantity": 0,
*       "price": 0,
*       "status": 0
*     }
*   ],
*   "totalPrice": 0,
*   "customerId": "string"
* }
* */

class Order {
    var orderId: String = ""
    var status: Int = 0
    var orderDate: String = ""
    var orderItems: List<OrderItem> = emptyList()
    var totalPrice: Double = 0.0
}

class OrderItem {
    var orderItemId: String = ""
    var productId: String = ""
    var productName: String = ""
    var quantity: Int = 0
    var price: Double = 0.0
    var status: Int = 0
}