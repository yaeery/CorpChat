package com.example.corpchat

import android.content.Intent
import android.content.SharedPreferences
import android.os.Bundle
import android.view.animation.Animation
import android.view.animation.AnimationUtils
import android.widget.ImageView
import androidx.activity.compose.setContent
import androidx.activity.enableEdgeToEdge
import androidx.appcompat.app.AppCompatActivity
import androidx.compose.material3.AlertDialog
import androidx.compose.material3.Button
import androidx.compose.material3.Text
import androidx.compose.runtime.mutableStateOf
import androidx.compose.ui.unit.sp
import androidx.core.view.ViewCompat
import androidx.core.view.WindowInsetsCompat
import kotlinx.coroutines.CoroutineScope

import androidx.activity.compose.setContent
import androidx.compose.material3.AlertDialog
import androidx.compose.material3.Button
import androidx.compose.material3.Text
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.ui.unit.sp

import android.graphics.PorterDuff
import android.view.View
import android.widget.EditText
import android.widget.TextView
import androidx.activity.ComponentActivity
import androidx.activity.compose.setContent
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.Surface
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.ui.Modifier
import androidx.compose.ui.tooling.preview.Preview
import com.example.corpchat.ui.theme.CorpChatTheme
import java.net.Socket
import android.app.Dialog
import androidx.appcompat.app.AlertDialog
import androidx.fragment.app.DialogFragment

var Saved_Key:String="1"
class MainActivity : AppCompatActivity() {
    private lateinit var  Rotate_Anim: Animation;
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        enableEdgeToEdge()
        setContentView(R.layout.activity_main)

        Rotate_Anim = AnimationUtils.loadAnimation( this,R.anim.rotate_intro )
        ViewCompat.setOnApplyWindowInsetsListener(findViewById(R.id.main)) { v, insets ->
            val systemBars = insets.getInsets(WindowInsetsCompat.Type.systemBars())
            v.setPadding(systemBars.left, systemBars.top, systemBars.right, systemBars.bottom)
            insets
        }
        //Save_Pref()
        Load_Pref()
    }
    fun Start_Rotate()
    {
        var Gear:ImageView = findViewById(R.id.gear);
        Gear.startAnimation(Rotate_Anim)
    }
    fun Verification()
    {
        android.os.Handler().postDelayed({
            val intent = Intent(this, Registration::class.java)
            startActivity(intent)
            finish()
       }, 4000)

    }

    fun Load_Pref()
    {
        Start_Rotate()
        val prefs = this.getSharedPreferences("preferences",MODE_PRIVATE)
        if((prefs.getString(Saved_Key,"")==null)||(prefs.getString(Saved_Key,"")==""))
        {
           // val intent = Intent(this, DialogsScroll::class.java)
            //startActivity(intent)
            Verification()
        }
        else
        {
            val intent = Intent(this, DialogsScroll::class.java)
            startActivity(intent)
            finish()
        }
    }
}