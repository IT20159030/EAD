package com.example.mobile

import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import com.example.mobile.ui.main.ProductViewFragment

class ProductViewActivity : AppCompatActivity() {

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_product_view)
        if (savedInstanceState == null) {
            supportFragmentManager.beginTransaction()
                .replace(R.id.container, ProductViewFragment.newInstance())
                .commitNow()
        }
    }
}