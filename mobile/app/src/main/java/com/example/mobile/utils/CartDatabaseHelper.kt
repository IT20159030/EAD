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
        const val COLUMN_PRODUCT_NAME = "product_name"
        const val COLUMN_QUANTITY = "quantity"
        const val COLUMN_TOTAL_PRICE = "total_price"
    }

    override fun onCreate(db: SQLiteDatabase?) {
        val createTableQuery = """
            CREATE TABLE $TABLE_CART (
                $COLUMN_ID INTEGER PRIMARY KEY AUTOINCREMENT,
                $COLUMN_PRODUCT_NAME TEXT,
                $COLUMN_QUANTITY INTEGER,
                $COLUMN_TOTAL_PRICE REAL
            )
        """.trimIndent()
        db?.execSQL(createTableQuery)
    }

    override fun onUpgrade(db: SQLiteDatabase?, oldVersion: Int, newVersion: Int) {
        db?.execSQL("DROP TABLE IF EXISTS $TABLE_CART")
        onCreate(db)
    }

    fun addToCart(productName: String, quantity: Int, totalPrice: Double): Long {
        val db = writableDatabase
        val values = ContentValues().apply {
            put(COLUMN_PRODUCT_NAME, productName)
            put(COLUMN_QUANTITY, quantity)
            put(COLUMN_TOTAL_PRICE, totalPrice)
        }
        return db.insert(TABLE_CART, null, values)
    }

    fun getAllCartItems(): List<CartItem> {
        val cartItems = mutableListOf<CartItem>()
        val db = readableDatabase
        val cursor: Cursor = db.query(TABLE_CART, null, null, null, null, null, null)
        while (cursor.moveToNext()) {
            val id = cursor.getInt(cursor.getColumnIndexOrThrow(COLUMN_ID))
            val productName = cursor.getString(cursor.getColumnIndexOrThrow(COLUMN_PRODUCT_NAME))
            val quantity = cursor.getInt(cursor.getColumnIndexOrThrow(COLUMN_QUANTITY))
            val totalPrice = cursor.getDouble(cursor.getColumnIndexOrThrow(COLUMN_TOTAL_PRICE))
            cartItems.add(CartItem(id, productName, quantity, totalPrice))
        }
        cursor.close()
        return cartItems
    }

    fun deleteCartItem(cartItemId: Int) {
        val db = writableDatabase
        db.delete(TABLE_CART, "$COLUMN_ID = ?", arrayOf(cartItemId.toString()))
    }
}
