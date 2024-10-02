package com.example.mobile.utils

import android.util.Base64
import org.json.JSONObject

fun getUserIdFromJWT(jwt: String): String? {
    try {
        // Split the JWT into its three parts
        val parts = jwt.split(".")
        if (parts.size != 3) {
            throw IllegalArgumentException("Invalid JWT token format")
        }

        // Get the payload (second part) and decode it from Base64 URL encoding
        val payload = parts[1]
        val decodedBytes = Base64.decode(payload, Base64.URL_SAFE)
        val decodedString = String(decodedBytes, Charsets.UTF_8)

        // Convert the decoded string into a JSON object
        val jsonObject = JSONObject(decodedString)

        // Extract the 'sub' claim (user ID) from the JSON object
        return jsonObject.getString("sub")

    } catch (e: Exception) {
        // Handle potential errors (e.g., decoding issues, invalid token format)
        e.printStackTrace()
        return null
    }
}