using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spaceship : MonoBehaviour{

    public GameObject[] Planets;
    public int simulationSpeed;

    private Vector3 velocity = new Vector3(-0.0085f, 0, 0);
    private Vector3 force;

    public float mass = 420E+3f;
    private float G = 6.67E-11f;

    public int orbitPoints;
    public Vector3[] orbit = new Vector3[500];

    private LineRenderer lineRenderer;

    public Vector3 angularMomentum;
    //public Vector3 nodeVector;
    private float µ;
    private float Energy;
    private Vector3 eccentricity;
    private float e; //Magnitude of the eccentricity vector
    private float r;
    private float theta;
    public float omega;
    private float circularPeri;

    public float period;
    public float timeSincePeriapsis;
    private float Me;
    private float a;
    private float E;
    private float rayon;

    public int iteration = 0;

    private delegate float Function(float E);

    private Function f;
    private Function df;

    // Start is called before the first frame update
    void Start(){

        ComputeCurrentOrbit();
        f = Kepler;
        df = dKepler;
        lineRenderer = GetComponent<LineRenderer>();
        timeSincePeriapsis = 0;

    }

    // Update is called once per frame
    void Update(){

        timeSincePeriapsis = (timeSincePeriapsis + simulationSpeed*Time.deltaTime) % period;

        Me = 2 * Mathf.PI * timeSincePeriapsis / period;

        E = Newton(Me, e, f, df, 1e-6f);

        theta = 2 * Mathf.Atan(Mathf.Sqrt((1 + e) / (1 - e)) * Mathf.Tan(E / 2));

        rayon = circularPeri * (1 / (1 + e * Mathf.Cos(theta)));
        rayon = rayon / 1000000;

        this.transform.position = new Vector3(rayon * Mathf.Cos(theta+omega), rayon * Mathf.Sin(theta+omega), 0);

        /*for (int j = 0; j < simulationSpeed; j++) {

            force = new Vector3(0, 0, 0);

            float deltaX = Planets[0].transform.position.x - this.transform.position.x;
            float deltaY = Planets[0].transform.position.y - this.transform.position.y;
            float norme = Mathf.Pow(Mathf.Pow(deltaX, 2) + Mathf.Pow(deltaY, 2), 0.5f);
            Vector3 Force_unit = new Vector3(deltaX / norme, deltaY / norme, 0);

                force = force + (G * Planets[0].GetComponent<Planet>().mass * mass / Mathf.Pow(1000000000 * norme, 2)) * Force_unit;

            velocity = velocity + force * Time.deltaTime / mass;
            Move(velocity);
        }*/

        lineRenderer.SetPositions(orbit);

    }

    void Move(Vector3 velocity) {

        this.transform.position = this.transform.position + (velocity*Time.deltaTime);

	}

    void ComputeCurrentOrbit() {

        this.transform.position = 1000000 * this.transform.position;
        velocity = 1000000 * velocity;

        µ = G * Planets[0].GetComponent<Planet>().mass;
        angularMomentum = mass * Vector3.Cross(this.transform.position, velocity);
        //nodeVector = Vector3.Cross(new Vector3(0, 0, 1), angularMomentum/mass);
        eccentricity = ((velocity.sqrMagnitude - (µ / this.transform.position.magnitude)) * this.transform.position - ((Vector3.Dot(this.transform.position, velocity)) * velocity)) / µ;
        e = eccentricity.magnitude;
        Energy = (velocity.sqrMagnitude / 2) - (µ / this.transform.position.magnitude);

        circularPeri = angularMomentum.sqrMagnitude / (Mathf.Pow(mass, 2) * µ);
        omega = Mathf.Atan2(eccentricity.y, eccentricity.x);

        bool hyperbola = false;

        if (e < 1) {
            a = -µ / (2 * Energy);
            period = 2 * Mathf.PI * Mathf.Sqrt(Mathf.Pow(a, 3) / µ);
        }

        for (int i = 0; i < orbitPoints; i++) {
            theta = (2 * Mathf.PI / 500) * i;
            r = circularPeri * (1 / (1 + e * Mathf.Cos(theta - omega)));
            r = r / 1000000;
            if (r > Planets[0].GetComponent<Planet>().SOISize) {
                orbit[i] = new Vector3(r * Mathf.Cos(theta), r * Mathf.Sin(theta), -20);
            } else if(r < 0){  //Hyperbole
                orbit[i] = new Vector3(r * Mathf.Cos(theta), r * Mathf.Sin(theta), -20);
                hyperbola = true;
            } else {
                orbit[i] = new Vector3(r * Mathf.Cos(theta), r * Mathf.Sin(theta), 1);
            }
        }

		if (hyperbola) {

            for(int i = 0; i < orbitPoints; i++) {
                if(orbit[i].z == -20) {
                    orbit[i] = orbit[i - 1 % orbitPoints];
				}
			}

		}

        this.transform.position = this.transform.position / 1000000;
        velocity = velocity / 1000000;

    }

    float Kepler(float E) {
        return E - e * Mathf.Sin(E);
	}

    float dKepler(float E) {
        return 1 - e * Mathf.Cos(E);
	}

    float Newton(float Me, float e, Function f, Function df, float tolerance) {

        float Em = 0;

        if(Me < Mathf.PI) {
            Em = Me + (e / 2);
		} else {
            Em = Me - (e / 2);
		}
        float ratio = 10;
        iteration = 0;
        while(Mathf.Abs(ratio) > tolerance && iteration < 10000) {
            ratio = (f(Em) - Me) / df(Em);
            Em = Em - ratio;
            iteration++;
		}

        return Em;

	}
}
