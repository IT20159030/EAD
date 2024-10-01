package com.example.mobile

import android.os.Bundle
import android.view.LayoutInflater
import android.view.Menu
import android.view.MenuItem
import android.view.View
import android.view.ViewGroup
import android.widget.PopupWindow
import com.google.android.material.bottomnavigation.BottomNavigationView
import androidx.appcompat.app.AppCompatActivity
import androidx.appcompat.widget.Toolbar
import androidx.navigation.fragment.NavHostFragment
import androidx.navigation.ui.AppBarConfiguration
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

        val navHostFragment = supportFragmentManager
            .findFragmentById(R.id.nav_host_fragment_activity_main) as NavHostFragment
        val navController = navHostFragment.navController
        val navView: BottomNavigationView = binding.navView

        // Setting up the Toolbar
        val toolbar = findViewById<Toolbar>(R.id.toolbar)
        setSupportActionBar(toolbar)

        // Setting up AppBarConfiguration for navigation
        val appBarConfiguration = AppBarConfiguration(
            setOf(R.id.navigation_home, R.id.navigation_cart, R.id.navigation_profile)
        )
        toolbar.setupWithNavController(navController, appBarConfiguration)

        // Control visibility of bottom navigation and toolbar based on destination
        navController.addOnDestinationChangedListener { _, destination, _ ->
            when (destination.id) {
                R.id.login_fragment, R.id.registerFragment -> {
                    navView.visibility = View.GONE
                    toolbar.visibility = View.GONE
                }
                R.id.navigation_home, R.id.navigation_cart, R.id.navigation_profile -> {
                    navView.visibility = View.VISIBLE
                    toolbar.visibility = View.VISIBLE
                }
                else -> {
                    navView.visibility = View.GONE
                    toolbar.visibility = View.VISIBLE
                }
            }
        }

        navView.setupWithNavController(navController)
    }

    // Inflate the toolbar menu
    override fun onCreateOptionsMenu(menu: Menu?): Boolean {
        menuInflater.inflate(R.menu.toolbar_menu, menu)
        return true
    }

    // Handle toolbar menu item clicks
    override fun onOptionsItemSelected(item: MenuItem): Boolean {
        return when (item.itemId) {
            R.id.action_notification -> {
                showNotificationPopup()
                true
            }
            else -> super.onOptionsItemSelected(item)
        }
    }

    // Show the notification popup on the right side of the toolbar
    private fun showNotificationPopup() {
        // Inflate the custom layout/view
        val inflater = getSystemService(LAYOUT_INFLATER_SERVICE) as LayoutInflater
        val popupView = inflater.inflate(R.layout.notification_popup, null)

        val popupWindow = PopupWindow(
            popupView,
            ViewGroup.LayoutParams.WRAP_CONTENT,
            ViewGroup.LayoutParams.WRAP_CONTENT,
            true
        )

        val toolbar = findViewById<Toolbar>(R.id.toolbar)
        popupView.measure(View.MeasureSpec.UNSPECIFIED, View.MeasureSpec.UNSPECIFIED)

        val xOffset = toolbar.width - popupView.measuredWidth
        popupWindow.showAsDropDown(toolbar, xOffset, 0)

        // Close the popup when clicked outside or elsewhere
        popupView.setOnTouchListener { _, _ ->
            popupWindow.dismiss()
            true
        }
    }
}
