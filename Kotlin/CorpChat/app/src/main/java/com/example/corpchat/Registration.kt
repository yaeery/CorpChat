package com.example.corpchat

import android.annotation.SuppressLint
import android.content.DialogInterface
import android.content.Intent
import android.content.SharedPreferences
import android.os.Bundle
import android.os.Handler
import android.os.Looper
import android.os.Message
import android.view.View
import android.widget.EditText
import android.widget.TextView
import androidx.activity.enableEdgeToEdge
import androidx.appcompat.app.AlertDialog
import androidx.appcompat.app.AppCompatActivity
import androidx.core.view.ViewCompat
import androidx.core.view.WindowInsetsCompat
import java.net.Socket
import java.net.SocketException
import kotlinx.coroutines.flow.*
import kotlinx.coroutines.*
class Registration : AppCompatActivity() {
    public var mainHandler:Handler? = null
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        enableEdgeToEdge()
        setContentView(R.layout.activity_registration)
        ViewCompat.setOnApplyWindowInsetsListener(findViewById(R.id.main)) { v, insets ->
            val systemBars = insets.getInsets(WindowInsetsCompat.Type.systemBars())
            v.setPadding(systemBars.left, systemBars.top, systemBars.right, systemBars.bottom)
            insets
        }
    }

    fun Incorect_L_P_D() {
        val bilder: AlertDialog.Builder = AlertDialog.Builder(this)
        bilder.setTitle("Внимание!")
        bilder.setMessage("Введите логин и пароль")
        bilder.setPositiveButton(
            "Ок", DialogInterface.OnClickListener(
                fun(dialogInterface: DialogInterface, i: Int): Unit = bilder.create().cancel())
        )

        val alertdialog: AlertDialog = bilder.create()

        alertdialog.show()
    }

    fun Incorect_L_P_D_A() {
        val bilder: AlertDialog.Builder = AlertDialog.Builder(this)
        bilder.setMessage("Неверный логин или пароль")
        bilder.setTitle("Внимание!")
        bilder.setPositiveButton(
            "Ок", DialogInterface.OnClickListener(
                fun(dialogInterface: DialogInterface, i: Int): Unit = bilder.create().cancel())
        )
        val alertdialog: AlertDialog = bilder.create()
        alertdialog.show()
    }

    public fun Internet_Problem() {
        val bilder: AlertDialog.Builder = AlertDialog.Builder(this)
        bilder.setTitle("Внимание!")
        bilder.setMessage("Сервер прилег")
        bilder.setPositiveButton(
            "Ок", DialogInterface.OnClickListener(
                fun(dialogInterface: DialogInterface, i: Int): Unit = bilder.create().cancel())
        )

        val alertdialog: AlertDialog = bilder.create()

        alertdialog.show()
    }

    fun Save_Pref(Input: String) {
        val prefs = this.getSharedPreferences(
            "preferences",
            MODE_PRIVATE
        )//getSharedPreferences("settings", Context.MODE_PRIVATE)
        var ed: SharedPreferences.Editor = prefs.edit()
        ed.putString(Saved_Key, Input)//пришедший код сюда вставить
        ed.apply()
    }

    fun Incorect_L_D() {
        val bilder: AlertDialog.Builder = AlertDialog.Builder(this)

        bilder.setMessage("Введите логин")
        bilder.setTitle("Внимание!")
        bilder.setPositiveButton(
            "Ок", DialogInterface.OnClickListener(
                fun(dialogInterface: DialogInterface, i: Int): Unit = bilder.create().cancel())
        )
        val alertdialog: AlertDialog = bilder.create()
        alertdialog.show()
    }

    fun Incorect_P_D() {
        val bilder: AlertDialog.Builder = AlertDialog.Builder(this)

        bilder.setMessage("Введите пароль")
        bilder.setTitle("Внимание!")
        bilder.setPositiveButton(
            "Ок", DialogInterface.OnClickListener(
                fun(dialogInterface: DialogInterface, i: Int): Unit = bilder.create().cancel())
        )
        val alertdialog: AlertDialog = bilder.create()
        alertdialog.show()
    }

    fun Sign_In_Button_Click(view: View) {
        val Name_Enter: EditText = findViewById(R.id.Name_Enter)
        val Password_Enter: EditText = findViewById(R.id.Password_Enter)
        var Is_No_Correct: Boolean = false
        if ((Name_Enter.getText().toString().equals("")) && ((Password_Enter.getText().toString()
                .equals("")) || (Password_Enter.getText().toString().length != 8))
        ) {
            Incorect_L_P_D()
            Is_No_Correct = true;
        } else if (Name_Enter.getText().toString().equals("")) {
            Is_No_Correct = true;
            Incorect_L_D()
        } else if ((Password_Enter.getText().toString().equals("")) || (Password_Enter.getText()
                .toString().length != 8)
        ) {
            Is_No_Correct = true;
            Incorect_P_D()

        }
    }


}
