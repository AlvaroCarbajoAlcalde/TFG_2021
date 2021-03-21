﻿using System;
using System.Data.OleDb;
using System.Drawing;
using System.Windows.Forms;

namespace Pokemon
{
    public partial class UC_ModificarPokemon : UserControl
    {
        public Entrenador entrenador;
        public int numPokemonEquipo, numPokemon, numMov1, numMov2, numMov3, numMov4, tipo1, tipo2;
        public String nombrePokedex;
        public Form_SeleccionEquipo seleccionEquipo;

        public void CargarPokemon(Pokemon pokemonACargar, int numPokemon)
        {
            SetPokemon(pokemonACargar.fkPokedex, nombrePokedex);
            numPokemonEquipo = numPokemon;
            textBoxApodo.Text = pokemonACargar.nombre;
            labelNombrePokemon.Text = pokemonACargar.nombrePokedex;
            textBoxAtq.Text = pokemonACargar.ataque + "";
            textBoxVel.Text = pokemonACargar.velocidad + "";
            textBoxDef.Text = pokemonACargar.defensa + "";
            textBoxEsp.Text = pokemonACargar.especial + "";
            textBoxHp.Text = pokemonACargar.vidaMax + "";
            textBoxAtq.Text = pokemonACargar.ataque + "";
            pictBoxPokemon.Image = pokemonACargar.imagenFront;
            nombrePokedex = pokemonACargar.nombrePokedex;
            if (pokemonACargar.mov1 != null)
            {
                numMov1 = pokemonACargar.mov1.idMovimiento;
                labelAtaque1.Text = pokemonACargar.mov1.nombre;
                picBoxCategoria1.BackgroundImage = Image.FromFile(@"Img\Categoria\" + pokemonACargar.mov1.categoria + ".gif");
                picBoxTipo1.BackgroundImage = Image.FromFile(@"Img\Tipes\" + (int)pokemonACargar.mov1.tipo + ".gif");
            }
            if (pokemonACargar.mov2 != null)
            {
                numMov2 = pokemonACargar.mov2.idMovimiento;
                labelAtaque2.Text = pokemonACargar.mov2.nombre;
                picBoxCategoria2.BackgroundImage = Image.FromFile(@"Img\Categoria\" + pokemonACargar.mov2.categoria + ".gif");
                picBoxTipo2.BackgroundImage = Image.FromFile(@"Img\Tipes\" + (int)pokemonACargar.mov2.tipo + ".gif");
            }
            if (pokemonACargar.mov3 != null)
            {
                numMov3 = pokemonACargar.mov3.idMovimiento;
                labelAtaque3.Text = pokemonACargar.mov3.nombre;
                picBoxCategoria3.BackgroundImage = Image.FromFile(@"Img\Categoria\" + pokemonACargar.mov3.categoria + ".gif");
                picBoxTipo3.BackgroundImage = Image.FromFile(@"Img\Tipes\" + (int)pokemonACargar.mov3.tipo + ".gif");
            }
            if (pokemonACargar.mov4 != null)
            {
                numMov4 = pokemonACargar.mov4.idMovimiento;
                labelAtaque4.Text = pokemonACargar.mov4.nombre;
                picBoxCategoria4.BackgroundImage = Image.FromFile(@"Img\Categoria\" + pokemonACargar.mov4.categoria + ".gif");
                picBoxTipo4.BackgroundImage = Image.FromFile(@"Img\Tipes\" + (int)pokemonACargar.mov4.tipo + ".gif");
            }
            if (pokemonACargar.esShiny)
                checkBoxShiny.Checked = true;
            else
                checkBoxShiny.Checked = false;
        }

        public UC_ModificarPokemon(int numPokemonEquipo, Entrenador entrenador, Form_SeleccionEquipo seleccionEquipo)
        {
            InitializeComponent();
            this.numPokemonEquipo = numPokemonEquipo;
            this.entrenador = entrenador;
            this.seleccionEquipo = seleccionEquipo;

            //Pokemons
            int contador = 0;
            OleDbConnection con = ConexionAccess.GetConexion();
            con.Open();
            OleDbCommand command = new OleDbCommand();
            command.Connection = con;
            command.CommandText = "select nombre, FK_TIPO1, FK_TIPO2, HP, ATAQUE, DEFENSA, ESPECIAL, VELOCIDAD from Pokemon order by id";
            OleDbDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                contador++;
                panelPokemons.Controls.Add(new Visualizador_Pokemon(contador, reader[0].ToString(), (int)reader[1], (int)reader[2], this, (int)reader[3], (int)reader[4], (int)reader[5], (int)reader[6], (int)reader[7]));
            }
            reader.Close();

            comboBoxMovimiento.SelectedIndex = 0;

            //Ataques
            OleDbCommand command2 = new OleDbCommand();
            command2.Connection = con;
            command2.CommandText = "select nombre, FK_TIPO, precision, potencia, descripcion, categoria, id, pp from movimiento where id<>0 order by fk_tipo, categoria, potencia, nombre";
            OleDbDataReader reader2 = command2.ExecuteReader();

            while (reader2.Read())
                panelDeSeleccionDeAtaque.Controls.Add(new Visualizador_Ataque((int)reader2[6], reader2[0].ToString(), (int)reader2[1], reader2[5].ToString(), reader2[4].ToString(), (int)reader2[3], (int)reader2[2], (int)reader2[7], this));

            reader2.Close();
            con.Close();
        }

        private void CheckBoxShiny_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxShiny.Checked)
                pictBoxPokemon.Image = Image.FromFile(@"Img\Sprites\Shiny\Front\" + nombrePokedex + ".gif");
            else
                pictBoxPokemon.Image = Image.FromFile(@"Img\Sprites\Front\" + nombrePokedex + ".gif");
        }

        private void BtnAceptar_Click(object sender, EventArgs e)
        {
            int hp, atq, def, esp, vel, id;
            if (int.TryParse(textBoxHp.Text, out hp) &&
                int.TryParse(textBoxAtq.Text, out atq) &&
                int.TryParse(textBoxEsp.Text, out esp) &&
                int.TryParse(textBoxDef.Text, out def) &&
                int.TryParse(textBoxVel.Text, out vel))
            {
                switch (numPokemonEquipo)
                {
                    case 0:
                        id = entrenador.auxPok1;
                        break;
                    case 1:
                        id = entrenador.auxPok2;
                        break;
                    case 2:
                        id = entrenador.auxPok3;
                        break;
                    case 3:
                        id = entrenador.auxPok4;
                        break;
                    case 4:
                        id = entrenador.auxPok5;
                        break;
                    case 5:
                        id = entrenador.auxPok6;
                        break;
                    default:
                        id = 0;
                        break;
                }
                //Insertamos valores en la tabla
                OleDbConnection con = ConexionAccess.GetConexion();
                con.Open();
                string query = "UPDATE almacenamiento SET apodo=@apodo, nivel=50, vida=@vida, ataque=@ataque, defensa=@defensa, " +
                    "especial=@especial, velocidad=@velocidad, fk_pokedex=@pokedex, movimiento_1=@mov1,  movimiento_2=@mov2,  " +
                    "movimiento_3=@mov3,  movimiento_4=@mov4, shiny=@shiny WHERE id = @id";
                OleDbCommand update = new OleDbCommand(query, con);
                update.Parameters.Add("@apodo", textBoxApodo.Text);
                update.Parameters.Add("@vida", hp);
                update.Parameters.Add("@ataque", atq);
                update.Parameters.Add("@defensa", def);
                update.Parameters.Add("@especial", esp);
                update.Parameters.Add("@velocidad", vel);
                update.Parameters.Add("@pokedex", numPokemon);
                update.Parameters.Add("@mov1", numMov1);
                update.Parameters.Add("@mov2", numMov2);
                update.Parameters.Add("@mov3", numMov3);
                update.Parameters.Add("@mov4", numMov4);
                update.Parameters.Add("@shiny", checkBoxShiny.Checked);
                update.Parameters.Add("@id", id);
                update.ExecuteNonQuery();
                con.Close();
                entrenador.equipo[numPokemonEquipo] = new Pokemon(id);
                switch (numPokemonEquipo)
                {
                    case 0:
                        seleccionEquipo.picBoxPkmn1.BackgroundImage = entrenador.equipo[numPokemonEquipo].icono;
                        entrenador.iconP1 = entrenador.equipo[numPokemonEquipo].icono;
                        break;
                    case 1:
                        seleccionEquipo.picBoxPkmn2.BackgroundImage = entrenador.equipo[numPokemonEquipo].icono;
                        entrenador.iconP2 = entrenador.equipo[numPokemonEquipo].icono;
                        break;
                    case 2:
                        seleccionEquipo.picBoxPkmn3.BackgroundImage = entrenador.equipo[numPokemonEquipo].icono;
                        entrenador.iconP3 = entrenador.equipo[numPokemonEquipo].icono;
                        break;
                    case 3:
                        seleccionEquipo.picBoxPkmn4.BackgroundImage = entrenador.equipo[numPokemonEquipo].icono;
                        entrenador.iconP4 = entrenador.equipo[numPokemonEquipo].icono;
                        break;
                    case 4:
                        seleccionEquipo.picBoxPkmn5.BackgroundImage = entrenador.equipo[numPokemonEquipo].icono;
                        entrenador.iconP5 = entrenador.equipo[numPokemonEquipo].icono;
                        break;
                    case 5:
                        seleccionEquipo.picBoxPkmn6.BackgroundImage = entrenador.equipo[numPokemonEquipo].icono;
                        entrenador.iconP6 = entrenador.equipo[numPokemonEquipo].icono;
                        break;
                }
                seleccionEquipo.inicio.InsertarDatos(entrenador);
                CargarPokemon(entrenador.equipo[numPokemonEquipo], numPokemonEquipo);
            }
            else
                MessageBox.Show("Los datos introducidos para el pokemon son incorrectos.");
        }

        public void SetPokemon(int numPokedex, String nombre)
        {
            nombrePokedex = nombre;
            numPokemon = numPokedex;
            pictBoxPokemon.Image = Image.FromFile(@"Img\Sprites\Front\" + nombre + ".gif");
            labelNombrePokemon.Text = nombre;
        }
    }
}
