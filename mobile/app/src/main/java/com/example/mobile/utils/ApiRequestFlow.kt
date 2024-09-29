package com.example.mobile.utils

import com.google.gson.Gson
import com.mrntlu.tokenauthentication.models.ErrorResponse
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.flow.Flow
import kotlinx.coroutines.flow.flow
import kotlinx.coroutines.flow.flowOn
import kotlinx.coroutines.withTimeoutOrNull
import retrofit2.Response

fun <T> apiRequestFlow(call: suspend () -> Response<T>): Flow<ApiResponse<T>> = flow {
    emit(ApiResponse.Loading)

    withTimeoutOrNull(20000L) {
        val response = call()

        try {
            if (response.isSuccessful) {
                response.body()?.let { data ->
                    emit(ApiResponse.Success(data))
                } ?: emit(ApiResponse.Failure("Response body is null", response.code())) // Handle null body
            } else {
                val errorBody = response.errorBody()
                if (errorBody != null) {
                    val errorJson = try {
                        errorBody.charStream().readText()
                    } catch (e: Exception) {
                        null
                    }

                    val parsedError = try {
                        Gson().fromJson(errorJson, ErrorResponse::class.java)
                    } catch (e: Exception) {
                        null
                    }

                    if (parsedError != null) {
                        emit(ApiResponse.Failure(parsedError.message, parsedError.code))
                    } else {
                        emit(ApiResponse.Failure(errorJson.toString(), response.code()))
                        println("Failed to parse error response with JSON: $errorJson")
                    }
                } else {
                    emit(ApiResponse.Failure("Error body is null", response.code()))
                }
            }
        } catch (e: Exception) {
            emit(ApiResponse.Failure(e.message ?: "An unknown error occurred", 400))
        }
    } ?: emit(ApiResponse.Failure("Timeout! Please try again.", 408))
}.flowOn(Dispatchers.IO)