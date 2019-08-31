using System;

namespace ClassicWowNeuralParasite
{
    class PlayerKalmanFilter
    {
        const int STATE_NUM = 9;

        const double MAP_POSITION_STD = 1e-7;
        const double MAP_VELOCITY_STD = 1e-5;
        const double ABS_VELOCITY_STD = 1e-1;
        const double HEADING_STD = 1e-6;
        const double SF_STD = 0.5;
        const double MAP_POSITION_PROCESS_NOISE = 1e-6;
        const double MAP_VELOCITY_PROCESS_NOISE = 1e-4;
        const double ABS_VELOCITY_PROCESS_NOISE = 1;
        const double HEADING_PROCESS_NOISE = 1e-5;
        const double SF_PROCESS_NOISE = 5;

        public KalmanMatrices Matrices = new KalmanMatrices();

        public PlayerKalmanFilter(double initXMap, double initYMap, double initHeading)
        {
            Matrices.X = new Vector(STATE_NUM);
            Matrices.X[0] = initXMap;
            Matrices.X[1] = initYMap;
            Matrices.X[2] = 0;
            Matrices.X[3] = 0;
            Matrices.X[4] = initHeading;
            Matrices.X[5] = 0;
            Matrices.X[6] = 0;
            Matrices.X[7] = 0;
            Matrices.X[8] = 0;

            Matrices.Xpred = new Vector(STATE_NUM);
            Matrices.P = new Matrix(STATE_NUM, STATE_NUM);
            Matrices.P[0, 0] = Math.Pow(MAP_POSITION_STD, 2);
            Matrices.P[1, 1] = Math.Pow(MAP_POSITION_STD, 2);
            Matrices.P[2, 2] = Math.Pow(MAP_VELOCITY_STD, 2);
            Matrices.P[3, 3] = Math.Pow(MAP_VELOCITY_STD, 2);
            Matrices.P[4, 4] = Math.Pow(HEADING_STD, 2);
            Matrices.P[5, 5] = Math.Pow(ABS_VELOCITY_STD, 2);
            Matrices.P[6, 6] = Math.Pow(ABS_VELOCITY_STD, 2);
            Matrices.P[7, 7] = Math.Pow(SF_STD, 2);
            Matrices.P[8, 8] = Math.Pow(SF_STD, 2);
            Matrices.Ppred = new Matrix(STATE_NUM, STATE_NUM);
            Matrices.PHI = new Matrix(STATE_NUM, STATE_NUM);
            Matrices.Q = new Matrix(STATE_NUM, STATE_NUM);
            Matrices.Q[0, 0] = Math.Pow(MAP_POSITION_PROCESS_NOISE, 2);
            Matrices.Q[1, 1] = Math.Pow(MAP_POSITION_PROCESS_NOISE, 2);
            Matrices.Q[2, 2] = Math.Pow(MAP_VELOCITY_PROCESS_NOISE, 2);
            Matrices.Q[3, 3] = Math.Pow(MAP_VELOCITY_PROCESS_NOISE, 2);
            Matrices.Q[4, 4] = Math.Pow(HEADING_PROCESS_NOISE, 2);
            Matrices.Q[5, 5] = Math.Pow(ABS_VELOCITY_PROCESS_NOISE, 2);
            Matrices.Q[6, 6] = Math.Pow(ABS_VELOCITY_PROCESS_NOISE, 2);
            Matrices.Q[7, 7] = Math.Pow(SF_PROCESS_NOISE, 2);
            Matrices.Q[8, 8] = Math.Pow(SF_PROCESS_NOISE, 2);
        }

        public void PredictState(double dt)
        {
            Matrices.PHI[0, 0] = 1.0;
            Matrices.PHI[0, 2] = dt;
            Matrices.PHI[1, 1] = 1.0;
            Matrices.PHI[1, 3] = dt;
            Matrices.PHI[2, 2] = 1.0;
            Matrices.PHI[3, 3] = 1.0;
            Matrices.PHI[4, 4] = 1.0;
            Matrices.PHI[5, 5] = 1.0;
            Matrices.PHI[6, 6] = 1.0;
            Matrices.PHI[7, 7] = 1.0;
            Matrices.PHI[8, 8] = 1.0;

            // Calculate predicted X and P 

            Matrices.Xpred[0] = Matrices.X[0] + Matrices.X[2] * dt;
            Matrices.Xpred[1] = Matrices.X[1] + Matrices.X[3] * dt;
            Matrices.Xpred[2] = Matrices.Xpred[2];
            Matrices.Xpred[3] = Matrices.Xpred[3];
            Matrices.Xpred[4] = Matrices.Xpred[4];
            Matrices.Xpred[5] = Matrices.Xpred[5];
            Matrices.Xpred[6] = Matrices.Xpred[6];
            Matrices.Xpred[7] = Matrices.Xpred[7];
            Matrices.Xpred[8] = Matrices.Xpred[8];

            Matrices.Ppred = (Matrices.PHI * Matrices.P * Matrices.PHI.Transpose()) + Matrices.Q;
        }

        public void ProcessMeasurements(double mapX, double mapY, double heading, double dt)
        {
            double xSpeed = (mapX - Matrices.X[0]) / dt;
            double ySpeed = (mapY - Matrices.X[1]) / dt;

            double speed = Math.Sqrt(xSpeed * xSpeed + ySpeed * ySpeed);

            Matrices.Z = null;
            Matrices.Z_hat = null;
            /*
            //if (speed > 0.12)
            {
                Matrices.Z = new Vector(9);
                Matrices.Z[0] = mapX;
                Matrices.Z[1] = mapY;
                Matrices.Z[2] = xSpeed;
                Matrices.Z[3] = ySpeed;
                Matrices.Z[4] = heading;

                Matrices.Z[5] = -Math.Sin(heading) * 7;
                Matrices.Z[6] = -Math.Cos(heading) * 7;

                Matrices.Z[7] = Matrices.Z[5] / Matrices.Z[2];
                Matrices.Z[8] = Matrices.Z[6] / Matrices.Z[3];

                Matrices.Z_hat = new Vector(9);
                Matrices.Z_hat[0] = Matrices.Xpred[0];
                Matrices.Z_hat[1] = Matrices.Xpred[1];
                Matrices.Z_hat[2] = Matrices.Xpred[2];
                Matrices.Z_hat[3] = Matrices.Xpred[3];
                Matrices.Z_hat[4] = Matrices.Xpred[4];
                Matrices.Z_hat[5] = Matrices.Xpred[5];
                Matrices.Z_hat[6] = Matrices.Xpred[6];
                Matrices.Z_hat[7] = Matrices.Xpred[7];
                Matrices.Z_hat[8] = Matrices.Xpred[8];
            }
            */
            //else
            {
                Matrices.Z = new Vector(7);
                Matrices.Z[0] = mapX;
                Matrices.Z[1] = mapY;
                Matrices.Z[2] = xSpeed;
                Matrices.Z[3] = ySpeed;
                Matrices.Z[4] = heading;
                Matrices.Z[5] = 0;
                Matrices.Z[6] = 0;

                Matrices.Z_hat = new Vector(7);
                Matrices.Z_hat[0] = Matrices.Xpred[0];
                Matrices.Z_hat[1] = Matrices.Xpred[1];
                Matrices.Z_hat[2] = Matrices.Xpred[2];
                Matrices.Z_hat[3] = Matrices.Xpred[3];
                Matrices.Z_hat[4] = Matrices.Xpred[4];
                Matrices.Z_hat[5] = Matrices.Xpred[5];
                Matrices.Z_hat[6] = Matrices.Xpred[6];

                Matrices.H = new Matrix(7, 9);
                Matrices.H[0, 0] = 1.0;
                Matrices.H[1, 1] = 1.0;
                Matrices.H[2, 2] = 1.0;
                Matrices.H[3, 3] = 1.0;
                Matrices.H[4, 4] = 1.0;
                Matrices.H[5, 5] = 1.0;
                Matrices.H[6, 6] = 1.0;

                Matrices.R = Matrix.Identity(7);
                Matrices.R[0, 0] = 1e-7 * 1e-7;
                Matrices.R[1, 1] = 1e-7 * 1e-7;
                Matrices.R[2, 2] = 1e-5 * 1e-5;
                Matrices.R[3, 3] = 1e-5 * 1e-5;
                Matrices.R[4, 4] = 1e-6 * 1e-6;
                Matrices.R[5, 5] = 1e-1 * 1e-1;
                Matrices.R[6, 6] = 1e-1 * 1e-1;
            }


        }

        public void ProcessCurrentState(double dt)
        {
            Matrix I = Matrix.Identity(Matrices.X.NumberOfElements);
            Matrix Iz = Matrix.Identity(Matrices.Z.NumberOfElements);

            // Calculate Gain

            //Matrices.G = Matrices.Ppred * Matrices.H.Transpose() * (Matrices.H * Matrices.Ppred * Matrices.H.Transpose() + Matrices.R).Inverse();
            Matrices.G = Matrices.Ppred * Matrices.H.Transpose() * !(Matrices.H * Matrices.Ppred * Matrices.H.Transpose() + Matrices.R);

            // Update State	

            Matrices.X = Matrices.Xpred + Matrices.G * (Matrices.Z - Matrices.Z_hat);

            Matrices.P = (I - Matrices.G * Matrices.H) * Matrices.Ppred * (I - Matrices.G * Matrices.H).Transpose() + Matrices.G * Matrices.R * Matrices.G.Transpose();
            Matrices.V = Matrices.Z - Matrices.H * Matrices.X;
            Matrices.Cv = (Iz - Matrices.H * Matrices.G) * Matrices.R;

        }

        public class KalmanMatrices
        {
            public Vector X;
            public Vector Xpred;
            public Matrix P;
            public Matrix Ppred;
            public Matrix PHI;
            //public Matrix B;
            public Matrix R;
            public Matrix Q;
            public Vector Z;
            public Vector Z_hat;
            public Matrix H;
            public Matrix G;
            public Vector V;
            public Matrix Cv;
        }
    }
}
