using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public enum CameraType { Intro, MoveX, DropBall, FollowBall, LookBall, Replay, Replay2, Anim, ReturnBall, MoveCam, ReactCam }
    public Ball ball;
    public Game game;
    public CameraType type;
    public float smoothPosSpeed = 0.25f;
    public float smoothRotSpeed = 10f;

    private Vector3 fallMove;
    private Coroutine replayCoroutine;

    void Update()
    {
        if (transform.position.y < GameObject.FindObjectOfType<PinSetter>().offsetY && type != CameraType.Anim && !game.isPin)
        {
            transform.position = new Vector3(transform.position.x, GameObject.FindObjectOfType<PinSetter>().offsetY, transform.position.z);
        }
        if (transform.position.z < -GameObject.FindObjectOfType<PinSetter>().offsetZ && ball.type == Ball.BallType.SpinBall)
        {
            type = CameraType.LookBall;
        }
    }

    void FixedUpdate ()
	{
        Vector3 desiredPosition;
        Vector3 smoothedPosition;
        if (type == CameraType.MoveX)
        {
            desiredPosition = new Vector3(ball.transform.position.x + GameObject.FindObjectOfType<PinSetter>().pos.x, GameObject.FindObjectOfType<PinSetter>().pos.y, GameObject.FindObjectOfType<PinSetter>().pos.z);
            smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothPosSpeed);
            transform.position = smoothedPosition;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(GameObject.FindObjectOfType<PinSetter>().rot * GameObject.FindObjectOfType<PinSetter>().rotSetX, 180, 0), smoothRotSpeed * Time.fixedDeltaTime);
        }
        else if (type == CameraType.DropBall)
        {
            desiredPosition = new Vector3(ball.transform.position.x, ball.transform.position.y + 45, ball.transform.position.z + 196);
            smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothPosSpeed);
            transform.position = smoothedPosition;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(GameObject.FindObjectOfType<PinSetter>().rot * GameObject.FindObjectOfType<PinSetter>().rotOffset, 180, 0), smoothRotSpeed * Time.fixedDeltaTime);
        }
        else if (type == CameraType.FollowBall)
        {
            desiredPosition = new Vector3(ball.transform.position.x + GameObject.FindObjectOfType<PinSetter>().offset.x, ball.transform.position.y + GameObject.FindObjectOfType<PinSetter>().offset.y, ball.transform.position.z + GameObject.FindObjectOfType<PinSetter>().offset.z);
            smoothedPosition = Vector3.Lerp(transform.position, new Vector3(desiredPosition.x, transform.position.y, desiredPosition.z), smoothPosSpeed);
            if (transform.position.y >= GameObject.FindObjectOfType<PinSetter>().offsetY && !game.isPin)
            {
                smoothedPosition = Vector3.Lerp(transform.position, new Vector3(desiredPosition.x, desiredPosition.y, desiredPosition.z), smoothPosSpeed);
            }
            transform.position = smoothedPosition;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(GameObject.FindObjectOfType<PinSetter>().rot * GameObject.FindObjectOfType<PinSetter>().rotOffset, 180, 0), smoothRotSpeed * Time.fixedDeltaTime);
        }
        else if (type == CameraType.LookBall)
        {
            desiredPosition = new Vector3(ball.transform.position.x + GameObject.FindObjectOfType<PinSetter>().offset.x, transform.position.y, transform.position.z);
            smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothPosSpeed);
            transform.position = smoothedPosition;
        }
        else if (type == CameraType.MoveCam)
        {
            transform.Translate(-fallMove.x * 0.125f * Time.fixedDeltaTime, 7.5f * Time.fixedDeltaTime, 30 * Time.fixedDeltaTime, Space.World);
        }
        else if (type == CameraType.Replay2)
        {
            transform.Translate(Vector3.forward * 60 * Time.fixedDeltaTime, Space.World);
        }
        else if (type == CameraType.ReturnBall)
        {
            transform.Translate(-10f * Time.fixedDeltaTime, -5f * Time.fixedDeltaTime, 500 * Time.fixedDeltaTime, Space.World);
            desiredPosition = new Vector3(ball.transform.position.x - transform.position.x, ball.transform.position.y - transform.position.y + 32, ball.transform.position.z - transform.position.z);
            Quaternion lookRotation = Quaternion.LookRotation(desiredPosition);
            transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, smoothRotSpeed * 2 * Time.fixedDeltaTime);
        }
    }

    public void Replay(int index)
    {
        type = CameraType.Replay;
        replayCoroutine = StartCoroutine(GameObject.FindObjectOfType<PinSetter>().replays[index].ReplayMove());
    }

    public void React(int index)
    {
        type = CameraType.ReactCam;
        transform.position = GameObject.FindObjectOfType<PinSetter>().reacts[index].position;
        transform.rotation = GameObject.FindObjectOfType<PinSetter>().reacts[index].rotation;
    }

    public void Reset()
    {
        if (replayCoroutine != null)
        {
            StopCoroutine(replayCoroutine);
        }
        Vector3 desiredPosition = new Vector3(ball.transform.position.x + GameObject.FindObjectOfType<PinSetter>().pos.x, GameObject.FindObjectOfType<PinSetter>().pos.y, GameObject.FindObjectOfType<PinSetter>().pos.z);
        transform.position = desiredPosition;
        transform.rotation = Quaternion.Euler(GameObject.FindObjectOfType<PinSetter>().rot * GameObject.FindObjectOfType<PinSetter>().rotSetX, 180, 0);
    }

    public void EndCam()
    {
        if (replayCoroutine != null)
        {
            StopCoroutine(replayCoroutine);
        }
        type = CameraType.ReactCam;
        transform.position = new Vector3(-GameObject.FindObjectOfType<PinSetter>().winPos.x, GameObject.FindObjectOfType<PinSetter>().winPos.y, GameObject.FindObjectOfType<PinSetter>().winPos.z);
        transform.rotation = Quaternion.Euler(-GameObject.FindObjectOfType<PinSetter>().winRot.x, 180 - GameObject.FindObjectOfType<PinSetter>().winRot.y, 0);
    }

    public void FallMove(Vector3 camMove)
    {
        fallMove = camMove;
    }
}
