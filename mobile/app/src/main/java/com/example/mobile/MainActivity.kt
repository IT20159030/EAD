package com.example.mobile

import android.os.Bundle
import android.view.View
import com.google.android.material.bottomnavigation.BottomNavigationView
import androidx.appcompat.app.AppCompatActivity
import androidx.navigation.fragment.NavHostFragment
import androidx.navigation.ui.setupWithNavController
import com.example.mobile.databinding.ActivityMainBinding
import dagger.hilt.android.AndroidEntryPoint

@AndroidEntryPoint
class MainActivity : AppCompatActivity() {

    private lateinit var binding: ActivityMainBinding

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        binding = ActivityMainBinding.inflate(layoutInflater)
        setContentView(binding.root)

        val navView: BottomNavigationView = binding.navView

        val navHostFragment = supportFragmentManager
            .findFragmentById(R.id.nav_host_fragment_activity_main) as NavHostFragment
        val navController = navHostFragment.navController

        navController.addOnDestinationChangedListener { _, destination, _ ->
            when (destination.id) {
                R.id.login_fragment, R.id.registerFragment -> {
                    navView.visibility = View.GONE
                    supportActionBar?.hide()
                }
                R.id.navigation_home, R.id.navigation_dashboard, R.id.navigation_profile -> {
                    navView.visibility = View.VISIBLE
                    supportActionBar?.show()
                }
                else -> {
                    supportActionBar?.show()
                }
            }
        }

        navView.setupWithNavController(navController)
    }
}
