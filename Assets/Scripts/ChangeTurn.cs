using Mirror;

public class ChangeTurn : NetworkBehaviour
{
    [SyncVar] //���������� ���� SyncVar ����� ���������� ������ �� �������, � �� �� �������
    NetworkIdentity IdentityOfDepartingPlayer; //������� ����������, � ������� ����� ��������� ������������ ������, ������� ������ ��� ������
}
