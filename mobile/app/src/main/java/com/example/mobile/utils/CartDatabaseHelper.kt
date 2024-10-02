package com.example.mobile.utils

import android.content.ContentValues
import android.content.Context
import android.database.Cursor
import android.database.sqlite.SQLiteDatabase
import android.database.sqlite.SQLiteOpenHelper
import com.example.mobile.data.model.CartItem

class CartDatabaseHelper(context: Context) : SQLiteOpenHelper(context, DATABASE_NAME, null, DATABASE_VERSION) {

    companion object {
        const val DATABASE_NAME = "cart.db"
        const val DATABASE_VERSION = 1
        const val TABLE_CART = "cart"
        const val COLUMN_ID = "id"
        const val COLUMN_PRODUCT_ID = "product_id"
        const val COLUMN_PRODUCT_NAME = "product_name"
        const val COLUMN_QUANTITY = "quantity"
        const val COLUMN_TOTAL_PRICE = "total_price"
        const val COLUMN_IMAGE_URL = "image_url"
    }

    override fun onCreate(db: SQLiteDatabase?) {
        val createTableQuery = """
            CREATE TABLE $TABLE_CART (
                $COLUMN_ID INTEGER PRIMARY KEY AUTOINCREMENT,
                $COLUMN_PRODUCT_ID TEXT,
                $COLUMN_PRODUCT_NAME TEXT,
                $COLUMN_QUANTITY INTEGER,
                $COLUMN_TOTAL_PRICE REAL,
                $COLUMN_IMAGE_URL TEXT
            )
        """.trimIndent()
        db?.execSQL(createTableQuery)
    }

    override fun onUpgrade(db: SQLiteDatabase?, oldVersion: Int, newVersion: Int) {
        db?.execSQL("DROP TABLE IF EXISTS $TABLE_CART")
        onCreate(db)
    }

    fun addToCart(productId: String, productName: String, quantity: Int, totalPrice: Double, imageUrl: String): Long {
        val db = writableDatabase
        val values = ContentValues().apply {
            put(COLUMN_PRODUCT_ID, productId)
            put(COLUMN_PRODUCT_NAME, productName)
            put(COLUMN_QUANTITY, quantity)
            put(COLUMN_TOTAL_PRICE, totalPrice)
            put(COLUMN_IMAGE_URL, imageUrl)
        }
        return db.insert(TABLE_CART, null, values)
    }

    fun getAllCartItems(): List<CartItem> {
        val cartItems = mutableListOf<CartItem>()
        val db = readableDatabase
        val cursor: Cursor = db.query(TABLE_CART, null, null, null, null, null, null)
        while (cursor.moveToNext()) {
            val id = cursor.getInt(cursor.getColumnIndexOrThrow(COLUMN_ID))
            val productId = cursor.getString(cursor.getColumnIndexOrThrow(COLUMN_PRODUCT_ID))
            val productName = cursor.getString(cursor.getColumnIndexOrThrow(COLUMN_PRODUCT_NAME))
            val quantity = cursor.getInt(cursor.getColumnIndexOrThrow(COLUMN_QUANTITY))
            val totalPrice = cursor.getDouble(cursor.getColumnIndexOrThrow(COLUMN_TOTAL_PRICE))
            val imageUrl = cursor.getString(cursor.getColumnIndexOrThrow(COLUMN_IMAGE_URL))
            cartItems.add(CartItem(id, productId, productName, quantity, totalPrice, imageUrl))
        }
        cursor.close()
        return cartItems
    }

    fun deleteCartItem(cartItemId: Int) {
        val db = writableDatabase
        db.delete(TABLE_CART, "$COLUMN_ID = ?", arrayOf(cartItemId.toString()))
    }

    fun clearCart() {
        val db = writableDatabase
        db.delete(TABLE_CART, null, null)
    }
}
