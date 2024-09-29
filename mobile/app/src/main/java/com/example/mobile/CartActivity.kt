package com.example.mobile

import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import com.example.mobile.ui.main.CartActivityFragment

class CartActivity : AppCompatActivity() {

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_cart_activity)
        if (savedInstanceState == null) {
            supportFragmentManager.beginTransaction()
                .replace(R.id.container, CartActivityFragment.newInstance())
                .commitNow()
        }
    }
}