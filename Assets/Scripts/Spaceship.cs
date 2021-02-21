using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spaceship : MonoBehaviour{

    public GameObject[] Planets;
    public int speed;

    private Vector3 velocity = new Vector3(0.0075f, 0, 0);
    private Vector3 force;

    public float mass = 420E+3f;
    private float G = 6.67E-11f;

    private Vector3 orbitPosition;
    private Vector3 orbitVelocity;
    private Vector3 orbitForce;
    public Vector3[] orbit = new Vector3[500];

    private LineRenderer lineRenderer;

    // Start is called before the first frame update
    void Start(){
        LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.positionCount = 500;
        lineRenderer.useWorldSpace = true;
    }

    // Update is called once per frame
    void Update(){

        LineRenderer lineRenderer = GetComponent<LineRenderer>();

        orbitPosition = this.transform.position;
        orbitPosition.z = -1;
        orbitVelocity = velocity;

        for (int j = 0; j < speed; j++) {

            force = new Vector3(0, 0, 0);

            for (int i = 0; i < Planets.Length; i++) {
                float deltaX = Planets[i].transform.position.x - this.transform.position.x;
                float deltaY = Planets[i].transform.position.y - this.transform.position.y;
                float norme = Mathf.Pow(Mathf.Pow(deltaX, 2) + Mathf.Pow(deltaY, 2), 0.5f);
                Vector3 Force_unit = new Vector3(deltaX / norme, deltaY / norme, 0);

                force = force + (G * Planets[i].GetComponent<Planet>().mass * mass / Mathf.Pow(1000000000 * norme, 2)) * Force_unit;

            }

            velocity = velocity + force * Time.deltaTime / mass;
            move(velocity);
        }


        for (int j = 0; j < 500; j++) {

            orbitForce = new Vector3(0, 0, 0);

            for (int i = 0; i < Planets.Length; i++) {
                float deltaX = Planets[i].transform.position.x - orbitPosition.x;
                float deltaY = Planets[i].transform.position.y - orbitPosition.y;
                float norme = Mathf.Pow(Mathf.Pow(deltaX, 2) + Mathf.Pow(deltaY, 2), 0.5f);
                Vector3 Force_unit = new Vector3(deltaX / norme, deltaY / norme, 0);

                orbitForce = orbitForce + (G * Planets[i].GetComponent<Planet>().mass * mass / Mathf.Pow(1000000000 * norme, 2)) * Force_unit;

            }

            orbitVelocity = orbitVelocity + orbitForce * 2500 * Time.deltaTime / mass;
            orbitPosition = orbitPosition + (orbitVelocity * 2500 * Time.deltaTime);
            orbit[j] = orbitPosition;

        }

        lineRenderer.SetPositions(orbit);

    }

    void move(Vector3 velocity) {

        this.transform.position = this.transform.position + (velocity*Time.deltaTime);

	}
}
