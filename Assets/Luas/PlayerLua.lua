xlua.private_accessible(CS.Player)

-- Start方法
function Start(obj)
    print("Player Start")

    local playerStateIdle = CS.PlayerStateIdle()
    obj:ChangeState(playerStateIdle)

    -- 修改初始化数据
    obj.moveSpeed = 5
    obj.jumpForce = 12.5
    obj.HP = 10
    obj.MP = 5
end

-- Update方法
function Updata(obj)
    if (obj.isPause == true)
    then
        return
    end

    obj.currentComboTime = obj.currentComboTime - CS.UnityEngine.Time.deltaTime
    obj.currentComboTime = CS.UnityEngine.Mathf.Max(obj.currentComboTime, -1)
    obj.currentComboWaitTime =  obj.currentComboWaitTime - CS.UnityEngine.Time.deltaTime
    obj.currentComboWaitTime = CS.UnityEngine.Mathf.Max(obj.currentComboWaitTime, -1)

    -- 调用该状态下的方法
    if (obj.playerState ~= nil)
    then
        obj.playerState:HandleInput(obj)
        obj.playerState:Update(obj)
    end

    -- 判断是否死亡
    if (obj:IsDead())
    then
        return
    end

    obj:CheckOnGround()
end

function LuaHarmer(obj, value)
    print("LuaHarmer")

    if (obj:IsDead())
    then
        return
    end

    CS.SoundManage:Player(CS.SoundManage.playerHit)

    obj:ChangeHP(value)
    if (obj:IsDead())
    then
        return
    end

    obj:ChangeState(CS.PlayerStateHarmed());
end