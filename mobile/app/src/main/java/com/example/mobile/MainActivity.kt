package com.example.mobile

import android.os.Bundle
import android.view.LayoutInflater
import android.view.Menu
import android.view.MenuItem
import android.view.View
import android.view.ViewGroup
import android.widget.PopupWindow
import android.widget.TextView
import androidx.activity.viewModels
import androidx.appcompat.app.AppCompatActivity
import androidx.appcompat.widget.Toolbar
import androidx.lifecycle.Observer
import androidx.navigation.fragment.NavHostFragment
import androidx.navigation.ui.AppBarConfiguration
import androidx.navigation.ui.setupWithNavController
import com.example.mobile.databinding.ActivityMainBinding
import com.example.mobile.data.model.Notification
import com.example.mobile.utils.TokenManager
import com.example.mobile.viewModels.NotificationViewModel
import com.google.android.material.bottomnavigation.BottomNavigationView
import dagger.hilt.android.AndroidEntryPoint
import javax.inject.Inject

@AndroidEntryPoint
class MainActivity : AppCompatActivity() {

    private lateinit var binding: ActivityMainBinding
    private val notificationViewModel: NotificationViewModel by viewModels()

    @Inject
    lateinit var tokenManager: TokenManager
    private var userID: String? = null 

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        binding = ActivityMainBinding.inflate(layoutInflater)
        setContentView(binding.root)

        tokenManager.userId.observe(this, Observer { userId ->
            userID = userId
        })

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
                fetchNotifications()
                true
            }
            else -> super.onOptionsItemSelected(item)
        }
    }

    private fun fetchNotifications() {
        userID?.let { id ->
            notificationViewModel.getNotifications(id).observe(this, Observer { notifications ->
                if (notifications != null && notifications.isNotEmpty()) {
                    showNotificationPopup(notifications)
                }
            })
        }
    }

    // Show the notification popup on the right side of the toolbar
    private fun showNotificationPopup(notifications: List<Notification>) {
        // Inflate the custom layout/view
        val inflater = getSystemService(LAYOUT_INFLATER_SERVICE) as LayoutInflater
        val popupView = inflater.inflate(R.layout.notification_popup, null)

        val notificationContainer = popupView.findViewById<ViewGroup>(R.id.notification_container)

        notificationContainer.removeAllViews()

        notifications.forEach { notification ->
            val notificationItem = TextView(this).apply {
                text = "${notification.message}"
                setPadding(16, 16, 16, 16)
                textAlignment = View.TEXT_ALIGNMENT_VIEW_START
                setOnClickListener {
                    notificationViewModel.markNotificationAsRead(notification.id)
                }
            }
            notificationContainer.addView(notificationItem)
        }

        val popupWindow = PopupWindow(
            popupView,
            ViewGroup.LayoutParams.WRAP_CONTENT,
            ViewGroup.LayoutParams.WRAP_CONTENT,
            true
        )

        val toolbar = findViewById<Toolbar>(R.id.toolbar)
        val location = IntArray(2)
        toolbar.getLocationOnScreen(location)

        val xOffset = location[0] + toolbar.width - popupView.measuredWidth
        popupWindow.showAsDropDown(toolbar, xOffset, 0)
        popupView.setOnTouchListener { _, _ ->
            popupWindow.dismiss()
            true
        }
    }
}
