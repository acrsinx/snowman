# 版本0剧情
## 开场
`file` `plot0_0.json`  
引导玩家开战。  
### `0`
`shot`
```
LoadCharacter(snowdog, dog, (-4, 0, -6));
LoadCharacter(snowman, snowman1, (-3, 0, -4));
LoadCharacter(snowman, snowman2, (-4, 0, -4));
LoadCharacter(snowman, snowman3, (-5, 0, -4));
SetCharacterPosition(Player, (-4, 0, -2));
CameraAnimation(2000, {
    SetCameraPositionAt((0, 1, 0));
    SetCameraRotation(0, 180)
}, {
    SetCameraPositionAt((0, 2.5, 0));
    SetCameraRotation(-5, 180);
    Goto(1)
});
```
### `1`
`caption`  
`雪狗`: `这雪熊又来侵扰斯诺镇了！`  
```
PlayAnimation(dog, fourFeet/talk);
CameraAnimation(2000, {
    LookAtCharacter(dog, 0.3, 1)
}, {
    LookAtCharacter(dog, 1.1, 1);
    SetCameraRotation(-5, -180);
});
AddTrigger(nextCaption, {
    Goto(1)
})
```
### `2`
`caption`  
`雪狗`: `雪人们，开战！`  
```
SetCameraPosition();
LoadCharacter(snowbear, bear1, (5, 1, 6));
LoadCharacter(snowbear, bear2, (3, 1, 4));
SetTaskName(击败雪熊。);
AddTrigger(bear1_die&&bear2_die, {
    SetTaskName(到雪狗处集合。);
    SetCharacterTarget(snowman1, (-3, 0, -4));
    SetCharacterTarget(snowman2, (-4, 0, -4));
    SetCharacterTarget(snowman3, (-5, 0, -4));
    AddTarget(dog, 3, {
        SetCharacterPosition(snowman1, (-3, 0, -4));
        SetCharacterPosition(snowman2, (-4, 0, -4));
        SetCharacterPosition(snowman3, (-5, 0, -4));
        SetCharacterPosition(Player, (-4, 0, -2));
        Jump(plot0/plot0_1.json)
    })
});
AddTrigger(nextCaption, {
    PauseAnimation(dog);
    Exit()
})
```
## 战斗结束
`file` `plot0_1.json`  
战斗结束，引导玩家输入名字。  
### `0`
`caption`  
`雪狗`: `雪人们，在我的带领下，我们胜利了！`  
```
LookAtCharacter(dog, 0.3, 1);
AddTrigger(nextCaption, {
    Goto(1)
})
```
### `1`
`caption`  
`雪狗`: `就像之前一样，这次的功劳就由我上报。`  
```
CameraAnimation(2000, {
    LookAtCharacter(dog, 0.3, 1.5)
}, {
    LookAtCharacter(dog, 0.3, 0.9)
});
AddTrigger(nextCaption, {
    Goto(1)
})
```
### `2`
`caption`  
`愤怒的雪人`: `可是……`
```
LookAtCharacter(snowman1, 0.7, 1.2);
AddTrigger(nextCaption, {
    Goto(1)
})
```
### `3`
`caption`  
`雪狗`: `“可是”什么“可是”，我说话你们听着就行了。回军营待命！`  
```
LookAtCharacter(dog, 0.3, 1);
AddTrigger(nextCaption, {
    Goto(1)
})
```
### `4`
`caption`  
`愤怒的雪人`: `每次胜利，都是雪狗请功去了。`  
```
LookAtCharacter(snowman1, 0.7, 1.2);
AddTrigger(nextCaption, {
    Goto(1)
})
```
### `5`
`caption`  
`冷静的雪人`: `是啊，它们总不把我们当成生物。`  
```
LookAtCharacter(snowman2, 0.7, 1.2);
AddTrigger(nextCaption, {
    Goto(1)
})
```
### `6`
`caption`  
`不屑的雪人`: `雪狗们自以为是，自矜攻伐。`  
```
LookAtCharacter(snowman3, 0.7, 1.2);
AddTrigger(nextCaption, {
    Goto(1)
})
```
### `7`
`caption`  
`众雪人`: `就是！就是！`  
```
CameraAnimation(2000, {
    LookAtCharacter(snowman1, 0.7, 1.2)
}, {
    LookAtCharacter(snowman3, 0.7, 1.2);
});
AddTrigger(nextCaption, {
    Goto(1)
})
```
### `8`
引导玩家输入自己的名字  
`caption`  
`你`: `（或许，成为一个独立自主的雪人要取一个名字。）`  
```
SetCameraPosition();
AddTrigger(playerNamed, {
    SetScene(base);
    Jump(plot0/plot0_2.json)
});
AddTrigger(nextCaption, {
    EnterName()
})
```
## 取完名字
`file` `plot0_2.json`  
玩家输入名字后，引导玩家离开雪地。  
### `0`
`caption`  
`愤怒的雪人`: `我们离开这里！看它那雪狗怎么办！`  
```
AddTrigger(nextCaption, {
    ShowChooses(走！, 我们确实该离开。, 这，这不对吧？);
    AddTrigger((choose1||choose2), {
        Goto(1)
    });
    AddTrigger(choose3, {
        Goto(2)
    })
})
```
### `1`
`caption`  
`冷静的雪人`: `北郊有一个荒地，我们或许可以建造属于自己的村子。`  
```
AddTrigger(nextCaption, {
    Exit()
})
```
### `2`
`caption`  
`%name%`: `（或许，再这样待下去，我们也不过是雪狗的工具罢。）`  
```
AddTrigger(nextCaption, {
    Goto(1)
})
```
### `3`
`caption`  
`%name%`: `我们走！`  
```
AddTrigger(nextCaption, {
    Goto(-2)
})
```
