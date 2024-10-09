package com.example.corpchat

import android.R.string
import android.annotation.SuppressLint
import android.os.Bundle
import android.view.View
import android.widget.Button
import android.widget.LinearLayout
import androidx.activity.enableEdgeToEdge
import androidx.appcompat.app.AppCompatActivity
import androidx.constraintlayout.widget.ConstraintLayout
import androidx.core.view.ViewCompat
import androidx.core.view.WindowInsetsCompat
import java.net.Socket


var client = Socket()

class DialogsScroll : AppCompatActivity() {
    @SuppressLint("UseCompatLoadingForDrawables")
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        client.close()
        enableEdgeToEdge()
        setContentView(R.layout.activity_dialogs_scroll)
        ViewCompat.setOnApplyWindowInsetsListener(findViewById(R.id.main)) { v, insets ->
            val systemBars = insets.getInsets(WindowInsetsCompat.Type.systemBars())
            v.setPadding(systemBars.left, systemBars.top, systemBars.right, systemBars.bottom)
            insets
        }
        val prefs = this.getSharedPreferences("preferences",MODE_PRIVATE)
        Work(prefs.getString("GD#" + Saved_Key,""))
        }

    fun Work(Input: String?)
    {
        try {
            client = Socket("192.168.235.219", 33023)
            client.getOutputStream().write(Input?.toByteArray());
            client.getOutputStream().flush();
            while (true)
            {
                val bytes = ByteArray(1024);
                val input = client.getInputStream();
                val bytesread = input.read(bytes);
                val Msg = String(bytes, 0, bytesread);
                if (Msg[0] != '~')
                {
                    if(Msg=="N")
                    {
                        break
                    }
                    else
                    {
                        var Prom: String = ""
                        var list: ArrayList<String> = arrayListOf()
                        for (i in 1  until Msg.length)
                        {
                            if (Msg[i] === '#')
                            {
                                list.add(Prom)
                                Prom = ""
                                continue
                            }
                            else
                            {
                                Prom += Msg[i]
                            }
                        }
                        list.add(Prom)
                        val LinearLayout_Dialogs: LinearLayout = findViewById(R.id.Layout_in_Scroll)
                        for(i in 0 until list.size)
                        {
                            var button: Button = Button(this)//,null,R.style.Btt)
                            button.text = list[i]
                            val buttonLayout = ConstraintLayout.LayoutParams(
                                ConstraintLayout.LayoutParams.FILL_PARENT,
                                200
                            )
                            button.setLayoutParams(buttonLayout);

                            button.textAlignment =View.TEXT_ALIGNMENT_CENTER
                            button.setTextColor(getColor(R.color.PaleTurquoise))
                            button.setBackgroundColor(getColor(R.color.RosyBrown))
                            button.background = getDrawable(R.drawable.border_dialogs)
                            button.setOnClickListener(View.OnClickListener
                            { // Обработка нажатия
                                button.setText("Добро пожаловать, " + button.getText())
                            })
                            button.setTextSize(25f)
                            LinearLayout_Dialogs.addView(button)
                        }
                        break
                    }
                }
            }
        }
        catch (exeption: Exception)
        {
            var x = exeption.message.toString()
        }
    }
    fun Finde_Button_Click(view: View) {}
}

