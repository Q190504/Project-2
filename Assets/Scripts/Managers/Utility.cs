using UnityEngine;

public class Utility
{
    public static Vector2 GetDirection(MoveDirectionType type, Vector2 moveDir)
    {
        if (moveDir == Vector2.zero)
            return Vector2.zero;

        moveDir = moveDir.normalized;

        switch (type)
        {
            case MoveDirectionType.Horizontal:
                return new Vector2(Mathf.Sign(moveDir.x), 0f);

            case MoveDirectionType.Vertical:
                return new Vector2(0f, Mathf.Sign(moveDir.y));

            case MoveDirectionType.Diagonal:
                float signX = Mathf.Sign(moveDir.x);
                float signY = Mathf.Sign(moveDir.y);
                return new Vector2(signX, signY).normalized;

            case MoveDirectionType.FourDirection:
                return Mathf.Abs(moveDir.x) > Mathf.Abs(moveDir.y)
                    ? new Vector2(Mathf.Sign(moveDir.x), 0f)
                    : new Vector2(0f, Mathf.Sign(moveDir.y));

            case MoveDirectionType.EightDirection:
                return GetEightDirection(moveDir);

            case MoveDirectionType.Free:
            default:
                return moveDir;
        }
    }

    private static Vector2 GetEightDirection(Vector2 input)
    {
        float angle = Vector2.SignedAngle(Vector2.right, input);
        angle = (angle + 360f) % 360f;

        if (angle < 22.5f || angle >= 337.5f) return Vector2.right;
        if (angle < 67.5f) return new Vector2(1, 1).normalized;
        if (angle < 112.5f) return Vector2.up;
        if (angle < 157.5f) return new Vector2(-1, 1).normalized;
        if (angle < 202.5f) return Vector2.left;
        if (angle < 247.5f) return new Vector2(-1, -1).normalized;
        if (angle < 292.5f) return Vector2.down;
        return new Vector2(1, -1).normalized;
    }
}
