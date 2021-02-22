using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spaceship : MonoBehaviour{

    public GameObject[] Planets;
    public int simulationSpeed;

    private Vector3 velocity = new Vector3(0.013f, 0, 0);
    private Vector3 force;

    public float mass = 420E+3f;
    private float G = 6.67E-11f;

    public int orbitPoints;
    public Vector3[] orbit = new Vector3[500];

    private LineRenderer lineRenderer;

    public Vector3 angularMomentum;
    //public Vector3 nodeVector;
    private float µ;
    public float Energy;
    public Vector3 eccentricity;
    public float e; //Magnitude of the eccentricity vector
    public float r;
    public float theta;
    public float omega;
    public float circularPeri;

    // Start is called before the first frame update
    void Start(){

        ComputeCurrentOrbit();

    }

    // Update is called once per frame
    void Update(){

        LineRenderer lineRenderer = GetComponent<LineRenderer>();

        for (int j = 0; j < simulationSpeed; j++) {

            force = new Vector3(0, 0, 0);

            float deltaX = Planets[0].transform.position.x - this.transform.position.x;
            float deltaY = Planets[0].transform.position.y - this.transform.position.y;
            float norme = Mathf.Pow(Mathf.Pow(deltaX, 2) + Mathf.Pow(deltaY, 2), 0.5f);
            Vector3 Force_unit = new Vector3(deltaX / norme, deltaY / norme, 0);

                force = force + (G * Planets[0].GetComponent<Planet>().mass * mass / Mathf.Pow(1000000000 * norme, 2)) * Force_unit;

            velocity = velocity + force * Time.deltaTime / mass;
            Move(velocity);
        }

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
}
