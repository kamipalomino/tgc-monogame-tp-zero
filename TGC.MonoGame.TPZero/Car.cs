using System;
using System.Collections.Generic;
using System.Text;

namespace TGC.MonoGame.TP
{
    class Car
    {
        private Vector3 Position {get; set; }= Vector3.Zero;
        private Model CarModel { get; set; }
        private Effect Effect { get; set; }
        private FollowCamera FollowCameraCar { get; set; }
        
        private float Timer {get; set; } =0f;

        private float Duracion {get; set; }= 3f; 
        public Matrix Wold {get; private set; }
        public const string ContentFolder3D = "Models/";
        public const string ContentFolderEffects = "Effects/";


        /** 
        * ? para mover de un lugar A a otro B 
        * ? Lerp(A,B,0)= A
        * ? Lerp (A,B,1)=B;
        * !  (1-C)*A + C*B  siendo C  entre 0 y 1
        */

        private Vector3 A {get; set; } = new Vector3(3f, 10f, 4f);
        private Vector3 B {get; set; } = new Vector3(6f, -5f, 2f);
        public void LoadContent(Effect effect,Model  model){
            // Cargo el modelo del logo.
            Model = model;
            Effect= effect;
        }
        public void acelero(GameTime gameTime){
            var elapsedTime = Convert.ToSingle(gameTime.ElapsedGameTime.TotalSeconds);
            Timer +=elapsedTime;
                Position = Vector3.Lerp(A,B,MathF.Min(Timer *0.1f * 2f,1f));
        }
        public void desacelero(GameTime gameTime){
            var elapsedTime = Convert.ToSingle(gameTime.ElapsedGameTime.TotalSeconds);
            Timer +=elapsedTime;
                Position = Vector3.Lerp(A,B,MathF.Min(Timer *0.1f / 2f,1f));
        }
        public void Update(GameTime gameTime){

            var elapsedTime = Convert.ToSingle(gameTime.ElapsedGameTime.TotalSeconds);
            Timer +=elapsedTime;
            
            /**
            * ? Se detiene despues cuando llega a los 3 seg
            * va a depender segun el tiempo de upgrade
            */
          //  if(Timer <= Duracion)
            //    Position += Vector3.Up * elapsedTime * 5f; 

            Position = Vector3.Lerp(A,B,MathF.Min(Timer *0.1f,1f));
            
            



            var quaternion = Quaternion.CreateFromAxisAngle(Vector3.Up,MathF.PI); //  timer 



            // new Vector3(0.2f,0.2f,0.2f)
            World = Matrix.CreateScale(0.2f) //puedo poner el timer aca
            //   * Matrix.CreateRotationY(MathF.PI)
                * Matrix.CreateFromQuaternion(quaternion)
                * Matrix.CreateTranslation(Position);
            // Basado en el tiempo que paso se va generando una rotacion.
            Rotation += Convert.ToSingle(gameTime.ElapsedGameTime.TotalSeconds);

            base.Update(gameTime);

        } 
        public void Draw(GameTime gameTime, Matrix view, Matrix projection){
            // Para dibujar le modelo necesitamos pasarle informacion que el efecto esta esperando.
            Effect.Parameters["View"].SetValue(view);
            Effect.Parameters["Projection"].SetValue(projection);
          //  Effect.Parameters["DiffuseColor"].SetValue(Color.DarkBlue.ToVector3());
            var rotationMatrix = Matrix.CreateRotationY(Rotation);

            foreach (var mesh in Model.Meshes)
            {
                World = mesh.ParentBone.Transform * rotationMatrix;
                Effect.Parameters["World"].SetValue(World);
                mesh.Draw();
            }
        } 
    }
}