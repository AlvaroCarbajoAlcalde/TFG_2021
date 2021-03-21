package com.example.mandopokemon;

import androidx.appcompat.app.AppCompatActivity;
import androidx.core.view.GestureDetectorCompat;

import android.content.ContentValues;
import android.database.Cursor;
import android.database.sqlite.SQLiteDatabase;
import android.os.Bundle;
import android.view.GestureDetector;
import android.view.MotionEvent;
import android.view.View;
import android.widget.ImageView;
import android.widget.TextView;

import com.example.mandopokemon.Modelo.PokedexPokemon;
import com.example.mandopokemon.SQL.ConexionSQLiteHelper;
import com.example.mandopokemon.Utiles.UtilImagenes;

import org.w3c.dom.Document;
import org.w3c.dom.Element;
import org.w3c.dom.Node;
import org.w3c.dom.NodeList;
import org.xml.sax.SAXException;

import java.io.IOException;

import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.DocumentBuilderFactory;
import javax.xml.parsers.ParserConfigurationException;

import static com.example.mandopokemon.SQL.ConexionSQLiteHelper.*;

public class PokedexActivity extends AppCompatActivity {

    private ConexionSQLiteHelper conn = new ConexionSQLiteHelper(this, "bd", null, 1);

    private TextView txtViewNombre, txtViewDescripcion, txtViewCategoria, txtViewPeso, txtViewAltura;
    private ImageView ivPokemon, ivTipo1, ivTipo2;

    private GestureDetectorCompat gestureDetector;

    private int pokemonActual;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_pokedex);

        initComponents();

        //Detector de gestos
        gestureDetector = new GestureDetectorCompat(this, new GestureListener());

        //Guarda los datos en la base de datos
        conn.onUpgrade(conn.getWritableDatabase(), 1, 2);
        leerDatosXML();

        pokemonActual = 1;
        setDatosPokemon(pokemonActual);
    }

    /**
     * Inicializa los componentes
     */
    private void initComponents() {
        ivPokemon = findViewById(R.id.imageViewPokemon);
        ivTipo1 = findViewById(R.id.ivTipo1);
        ivTipo2 = findViewById(R.id.ivTipo2);
        txtViewNombre = findViewById(R.id.txtViewNombre);
        txtViewDescripcion = findViewById(R.id.txtViewDescripcion);
        txtViewCategoria = findViewById(R.id.txtViewCategoria);
        txtViewPeso = findViewById(R.id.txtViewPeso);
        txtViewAltura = findViewById(R.id.txtViewAltura);
    }

    /**
     * Lee los datos del xml para insertarlos en la base de datos
     */
    public void leerDatosXML() {
        PokedexPokemon pokemon = new PokedexPokemon();
        try {
            DocumentBuilderFactory factory = DocumentBuilderFactory.newInstance();
            DocumentBuilder builder = factory.newDocumentBuilder();
            // Parseamos el documento y lo almacenamos en un objeto Document
            Document doc = builder.parse(getResources().openRawResource(R.raw.pokedex));

            // Obtenemos el elemento raiz del documento, pokemons
            Element raiz = doc.getDocumentElement();

            // Obtenemos todos los elementos llamados pokemon, que cuelgan de la raiz
            NodeList items = raiz.getElementsByTagName("Pokemon");

            // Recorremos todos los elementos obtenidos
            for (int i = 0; i < items.getLength(); i++) {
                Node nodoPokemon = items.item(i);

                // Recorremos todos los hijos que tenga el nodo pokemon
                for (int j = 0; j < nodoPokemon.getChildNodes().getLength(); j++) {
                    Node nodoActual = nodoPokemon.getChildNodes().item(j);

                    // Compruebo si es un elemento
                    if (nodoActual.getNodeType() == Node.ELEMENT_NODE) {
                        switch (nodoActual.getNodeName().toLowerCase()) {
                            case "idpok":
                                pokemon.setId(Integer.parseInt(nodoActual.getChildNodes().item(0).getNodeValue()));
                                break;
                            case "nombre":
                                pokemon.setNombre(nodoActual.getChildNodes().item(0).getNodeValue());
                                break;
                            case "tipo1":
                                pokemon.setTipo1(Integer.parseInt(nodoActual.getChildNodes().item(0).getNodeValue()));
                                break;
                            case "tipo2":
                                pokemon.setTipo2(Integer.parseInt(nodoActual.getChildNodes().item(0).getNodeValue()));
                                break;
                            case "categoria":
                                pokemon.setCategoria(nodoActual.getChildNodes().item(0).getNodeValue());
                                break;
                            case "descripcion":
                                pokemon.setDescripcion(nodoActual.getChildNodes().item(0).getNodeValue());
                                break;
                            case "peso":
                                pokemon.setPeso(nodoActual.getChildNodes().item(0).getNodeValue());
                                break;
                            case "altura":
                                pokemon.setAltura(nodoActual.getChildNodes().item(0).getNodeValue());
                                break;
                        }
                    }
                }
                //Insertamos esos datos
                insertarDatos(pokemon);
            }

        } catch (ParserConfigurationException | SAXException | IOException e) {
            System.out.println("ERROR al leer XML: " + e.getMessage());
        }
    }

    /**
     * Inserta datos en la base de datos.
     *
     * @param pokemon Datos a insertar
     */
    public void insertarDatos(PokedexPokemon pokemon) {
        try {
            SQLiteDatabase db = conn.getWritableDatabase();
            ContentValues values = new ContentValues();

            values.put(CAMPO_POKEMON_ID, pokemon.getId());
            values.put(CAMPO_POKEMON_NOMBRE, pokemon.getNombre());
            values.put(CAMPO_POKEMON_CATEGORIA, pokemon.getCategoria());
            values.put(CAMPO_POKEMON_DESCRIPCION, pokemon.getDescripcion());
            values.put(CAMPO_POKEMON_ALTURA, pokemon.getAltura());
            values.put(CAMPO_POKEMON_PESO, pokemon.getPeso());
            values.put(CAMPO_POKEMON_TIPO_1, pokemon.getTipo1());
            values.put(CAMPO_POKEMON_TIPO_2, pokemon.getTipo2());

            db.insert(NOMBRE_TABLA_POKEMON, CAMPO_POKEMON_ID, values);
            db.close();
        } catch (Exception e) {
            System.out.println("Error insert: " + e.getMessage());
        }
    }

    /**
     * Cambia los datos de la vista para ajustarlos al pokeomon
     *
     * @param id id del pokemon para realizar la consulta
     */
    public void setDatosPokemon(int id) {
        //Realizamos consulta
        SQLiteDatabase db = conn.getReadableDatabase();
        String consulta = String.format("SELECT * FROM %s WHERE %s=%d", NOMBRE_TABLA_POKEMON, CAMPO_POKEMON_ID, id);
        Cursor c = db.rawQuery(consulta, null);

        if (c.moveToFirst()) {
            //Establecemos los datos
            txtViewNombre.setText(c.getString(c.getColumnIndex(CAMPO_POKEMON_NOMBRE)));
            txtViewDescripcion.setText(c.getString(c.getColumnIndex(CAMPO_POKEMON_DESCRIPCION)));
            ivPokemon.setImageResource(UtilImagenes.getIdRecursoPokemon(c.getInt(c.getColumnIndex(CAMPO_POKEMON_ID))));
            ivTipo1.setImageResource(UtilImagenes.getIdRecursoTipo(c.getInt(c.getColumnIndex(CAMPO_POKEMON_TIPO_1))));
            txtViewAltura.setText("Altura: " + c.getString(c.getColumnIndex(CAMPO_POKEMON_ALTURA)));
            txtViewPeso.setText("Peso: " + c.getString(c.getColumnIndex(CAMPO_POKEMON_PESO)));
            txtViewCategoria.setText("Pokemon " + c.getString(c.getColumnIndex(CAMPO_POKEMON_CATEGORIA)));

            //Si solo hay un tipo invisibilizamos el otro
            int tipo2 = UtilImagenes.getIdRecursoTipo(c.getInt(c.getColumnIndex(CAMPO_POKEMON_TIPO_2)));
            if (tipo2 != Integer.MAX_VALUE) {
                ivTipo2.setImageResource(tipo2);
                ivTipo2.setVisibility(View.VISIBLE);
            } else {
                ivTipo2.setVisibility(View.GONE);
            }
        }

        db.close();
    }

    private class GestureListener extends GestureDetector.SimpleOnGestureListener {

        @Override
        //Cuando se hace un gesto se llama a este metodo
        public boolean onFling(MotionEvent e1, MotionEvent e2, float velocityX, float velocityY) {
            //Comprobamos hacia que lado se hizo el gesto.
            if (velocityX < 0) {
                pokemonActual++;
                if (pokemonActual > 493) pokemonActual = 1;
            } else {
                pokemonActual--;
                if (pokemonActual < 1) pokemonActual = 493;
            }
            setDatosPokemon(pokemonActual);
            return super.onFling(e1, e2, velocityX, velocityX);
        }

    }

    @Override
    public boolean onTouchEvent(MotionEvent event) {
        gestureDetector.onTouchEvent(event);
        return super.onTouchEvent(event);
    }
}