package com.example.mobile.ui.vendorView

import android.annotation.SuppressLint
import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.RatingBar
import android.widget.TextView
import androidx.navigation.NavController
import androidx.recyclerview.widget.RecyclerView
import com.example.mobile.R
import com.example.mobile.dto.Vendor
import java.util.Locale

/*
* A RecyclerView adapter for displaying a list of vendors.
* Displays the vendor name, city, rating, and rating count.
* */

class VendorAdapter(private var vendors: List<Vendor>, private var navController: NavController) :
    RecyclerView.Adapter<VendorAdapter.VendorViewHolder>() {

    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): VendorViewHolder {
        val view = LayoutInflater.from(parent.context).inflate(R.layout.vendor_item, parent, false)
        return VendorViewHolder(view)
    }

    override fun onBindViewHolder(holder: VendorViewHolder, position: Int) {
        val vendor = vendors[position]
        holder.bind(vendor, navController)
    }

    override fun getItemCount(): Int {
        return vendors.size
    }

    @SuppressLint("NotifyDataSetChanged")
    fun updateList(newVendors: List<Vendor>) {
        vendors = newVendors
        notifyDataSetChanged()
    }

    class VendorViewHolder(itemView: View) : RecyclerView.ViewHolder(itemView) {
        private val vendorNameTextView: TextView = itemView.findViewById(R.id.vendor_name)
        private val vendorCityTextView: TextView = itemView.findViewById(R.id.vendor_city)
        private val vendorRatingTextView: RatingBar = itemView.findViewById(R.id.vendor_rating)
        private val vendorRatingCountTextView: TextView = itemView.findViewById(R.id.vendor_rating_count)
        private val vendorCardView: View = itemView.findViewById(R.id.vendor_item_card)

        fun bind(vendor: Vendor, navController: NavController) {
            vendorNameTextView.text = vendor.vendorName
            vendorCityTextView.text = vendor.vendorCity
            vendorRatingTextView.rating = vendor.vendorRating.toFloat()
            vendorRatingCountTextView.text = String.format(Locale.getDefault(), "(%d)", vendor.vendorRatingCount)

            vendorCardView.setOnClickListener {
                val bundle = Bundle().apply {
                    putString("vendorId", vendor.vendorId)
                    putString("vendorName", vendor.vendorName)
                    putString("vendorCity", vendor.vendorCity)
                    putFloat("vendorRating", vendor.vendorRating.toFloat())
                    putInt("vendorRatingCount", vendor.vendorRatingCount)
                }
                // Navigate to vendorView details
                navController.navigate(R.id.action_navigation_home_to_viewVendor, bundle)
            }
        }
    }
}
