# 版本0剧情
## 开场
`file` `plot0_0.json`  
引导玩家开战。  
### `0`
`雪狗`: `这雪熊又来侵扰斯诺镇了！`  
`caption`  
```
LoadCharacter(snowdog, dog, (-4, 0, -6));
LoadCharacter(snowman, snowman1, (-3, 0, -4));
LoadCharacter(snowman, snowman2, (-4, 0, -4));
LoadCharacter(snowman, snowman3, (-5, 0, -4));
PlayerTo(-4, 0, -2);
PlayAnimation(dog, talk);
LookAtCharacter(dog, 0.3, 1)
```
```
LookAtCharacter(dog, 1.1, 1)
PauseAnimation(dog);
Goto(1)
```
### `1`
`雪狗`: `雪人们，开战！`  
`caption`  
```
SetCameraPosition();
```
```
LoadCharacter(snowbear, bear1, (5, 1, 6));
LoadCharacter(snowbear, bear2, (3, 1, 4));
AddTrigger(bear1_die&&bear2_die, plot0/plot0_1.json);
Exit(0)
```
## 战斗结束
`file` `plot0_1.json`  
战斗结束，引导玩家输入名字。  
### `0`
`雪狗`: `雪人们，在我的带领下，我们胜利了！`  
`caption`  
```
LookAtCharacter(dog, 0.3, 1)
```
```
Goto(1)
```
### `1`
`雪狗`: `就像之前一样，这次的功劳就由我上报。`  
`caption`  
```
LookAtCharacter(dog, 0.3, 2)
```
```
Goto(1)
```
### `2`
`愤怒的雪人`: `可是……`
`caption`  
```
LookAtCharacter(snowman1, 0.7, 1.2)
```
```
Goto(1)
```
### `3`
`雪狗`: `“可是”什么“可是”，我说话你们听着就行了。`  
`caption`  
```
LookAtCharacter(dog, 0.3, 1)
```
```
Goto(1)
```
### `4`
`愤怒的雪人`: `每次胜利，都是雪狗请功去了。`  
`caption`  
```
LookAtCharacter(snowman1, 0.7, 1.2)
```
```
Goto(1)
```
### `5`
`冷静的雪人`: `是啊，它们总不把我们当成生物。`  
`caption`  
```
LookAtCharacter(snowman2, 0.7, 1.2)
```
```
Goto(1)
```
### `6`
`不屑的雪人`: `雪狗们自以为是，自矜攻伐。`  
`caption`  
```
LookAtCharacter(snowman3, 0.7, 1.2)
```
```
Goto(1)
```
### `7`
`众雪人`: `就是！就是！`  
`caption`  
```
LookAtCharacter(snowman1, 0.7, 1.2)
```
```
LookAtCharacter(snowman3, 0.7, 1.2);
Goto(1)
```
### `8`
引导玩家输入自己的名字  
`你`: `（或许，成为一个独立自主的雪人要取一个名字。）`  
`caption`  
```
SetCameraPosition()
```
```
AddTrigger(playerNamed, plot0/plot0_2.json);
Exit(0)
```
## 取完名字
`file` `plot0_2.json`  
玩家输入名字后，引导玩家离开雪地。  
### `0`
`愤怒的雪人`: `我们离开这里！看它那雪狗怎么办！`  
`choose`  
```
```
`走！`  
```
Goto(1)
```
`我们确实该离开。`  
```
Goto(1)
```
`这，这不对吧？`  
```
Goto(2)
```
`endChoose`
### `1`
`冷静的雪人`: `北郊有一个荒地，我们或许可以建造属于自己的村子。`  
`caption`  
```
```
```
Exit(0)
```
### `2`
`%name%`: `（或许，再这样待下去，我们也不过是雪狗的工具罢。）`  
`caption`  
```
```
```
Goto(1)
```
### `3`
`%name%`: `我们走！`  
`caption`  
```
```
```
Goto(1)
```
