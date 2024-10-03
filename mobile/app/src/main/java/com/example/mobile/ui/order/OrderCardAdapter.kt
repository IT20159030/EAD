package com.example.mobile.ui.order

import android.animation.Animator
import android.animation.AnimatorListenerAdapter
import android.animation.ValueAnimator
import android.content.Context
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.ArrayAdapter
import android.widget.Button
import android.widget.LinearLayout
import android.widget.ListView
import android.widget.TextView
import androidx.recyclerview.widget.RecyclerView
import com.example.mobile.R
import com.example.mobile.dto.OrderResponse
import com.example.mobile.utils.resolveOrderStatus
import java.util.Locale

class OrderCardAdapter(
    private val orders: List<OrderResponse>,
    private val context: Context,
    private val onCancelOrder: (String) -> Unit
) : RecyclerView.Adapter<OrderCardAdapter.OrderViewHolder>() {

    class OrderViewHolder(view: View) : RecyclerView.ViewHolder(view) {
        val orderId: TextView = view.findViewById(R.id.order_item_card_order_id)
        val date: TextView = view.findViewById(R.id.order_item_card_order_date)
        val status: TextView = view.findViewById(R.id.order_item_card_status)
        val orderItemToggle: TextView = view.findViewById(R.id.order_item_card_toggle)
        val orderItemListView: ListView = view.findViewById(R.id.order_item_card_listview)
        val price: TextView = view.findViewById(R.id.order_item_card_price)
        val cancelButton: Button = view.findViewById(R.id.order_item_card_cancel_button)
    }

    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): OrderViewHolder {
        val view = LayoutInflater.from(parent.context)
            .inflate(R.layout.order_item_card, parent, false)
        return OrderViewHolder(view)
    }

    override fun onBindViewHolder(holder: OrderViewHolder, position: Int) {
        val order = orders[position]

        // Bind data to the views
        holder.orderId.text = String.format(Locale.getDefault(),
            context.getString(R.string.orderid_s), order.orderId)
        holder.date.text = String.format(Locale.getDefault(),
            context.getString(R.string.date_s), order.orderDate)
        holder.status.text = resolveOrderStatus(order.status)
        holder.price.text = String.format(Locale.getDefault(), context.getString(R.string.s_2f),
            context.getString(R.string.currency), order.totalPrice)

        // Set the adapter for the ListView
        val adapter = ArrayAdapter(context, android.R.layout.simple_list_item_1, order.orderItems.map {
            "${it.productName} x ${it.quantity} - ${resolveOrderStatus(it.status)}"
        })
        holder.orderItemListView.adapter = adapter

        // Set click listener for the toggle button
        holder.orderItemToggle.setOnClickListener {
            if (holder.orderItemListView.visibility == View.GONE) {
                expand(holder.orderItemListView)
                holder.orderItemToggle.text = context.getString(R.string.hide_items)
            } else {
                collapse(holder.orderItemListView)
                holder.orderItemToggle.text = context.getString(R.string.show_items)
            }
        }

        // Set cancel button listener
        holder.cancelButton.setOnClickListener {
            onCancelOrder(order.id)
        }
    }

    override fun getItemCount(): Int = orders.size

    private fun expand(view: View) {
        view.measure(LinearLayout.LayoutParams.MATCH_PARENT, LinearLayout.LayoutParams.WRAP_CONTENT)
        val targetHeight = view.measuredHeight

        view.layoutParams.height = 0
        view.visibility = View.VISIBLE
        val animation = ValueAnimator.ofInt(0, targetHeight)
        animation.addUpdateListener { valueAnimator ->
            view.layoutParams.height = valueAnimator.animatedValue as Int
            view.requestLayout()
        }
        animation.duration = 300
        animation.start()
    }

    private fun collapse(view: View) {
        val initialHeight = view.measuredHeight
        val animation = ValueAnimator.ofInt(initialHeight, 0)
        animation.addUpdateListener { valueAnimator ->
            view.layoutParams.height = valueAnimator.animatedValue as Int
            view.requestLayout()
        }
        animation.addListener(object : AnimatorListenerAdapter() {
            override fun onAnimationEnd(animation: Animator) {
                view.visibility = View.GONE
            }
        })
        animation.duration = 300
        animation.start()
    }
}