ここは私のvrプロゼット”VRDEFNSE”です。
VR環境でDEFENSEゲームをできます。

VRDefense - VR防衛ゲーム

1. プロジェクト概要

  VRDefenseは、VR環境でプレイできるタワーディフェンスゲームです。本プロジェクトでは、プレイヤーが爆弾を投げたり、ドローンと戦ったり、ワープで瞬間移動するなどのアクションを実現しています。

2. 使用された主要技術

  本プロジェクトでは以下の技術を使用しました。

2.1 爆弾システム (Bomb System)

  Bomb.cs & BombManager.cs

  爆弾を設置し、特定の範囲にダメージを与えるシステム

  爆発エフェクトと音響効果を実装

  BombManager を用いて複数の爆弾を管理

2.2 ドローンAIおよび管理システム (Drone AI & Manager)
  
  DroneAI.cs & DroneManager.cs

  NavMeshAgent を用いた移動と攻撃システム

  プレイヤーやタワーに向かって移動し、特定範囲内で攻撃

  被弾時に爆発するエフェクトを実装

  ランダムなタイミングでドローンを生成する DroneManager

2.3 銃撃システム (Gun System)

  Gun.cs

  OVRInput を活用し、VRコントローラーで射撃を実装

  レイキャストを用いたヒット判定とドローンへのダメージ処理

  発砲時のエフェクトと音響効果を追加

2.4 物体を掴む＆投げるシステム (Grabbing System)

  Grab.cs & GrabO.cs

  プレイヤーが特定のオブジェクト（爆弾など）を掴み、投げることが可能

  SphereCast を用いた遠距離グラブ機能を実装

  LineRenderer を用いて掴めるオブジェクトの可視化を行う

2.5 VR移動システム (Teleportation System)

  TeleportStraight.cs, TeleportCurve.cs, TeleportCurved.cs

  LineRenderer を用いたテレポート範囲の可視化

  Physics.Raycast により適切な地形を判定し、瞬間移動を実施

  Warp モードではモーションブラーを利用して移動演出を強化

2.6 プレイヤー移動とジャンプ (Player Movement)

  PlayerMove.cs

  CharacterController を用いたVRプレイヤーの移動とジャンプ機能

  OVRInput を活用し、VRコントローラーでジャンプが可能

  カメラの向きに基づいて移動できるシステムを導入

3. 使用されたスクリプト

  スクリプト名

  機能概要

  Bomb.cs

  爆弾の爆発処理を管理

  BombManager.cs

  爆弾の設置および再生成を管理

  DroneAI.cs

  ドローンのAI（移動・攻撃・破壊）

  DroneManager.cs

  ドローンのランダムスポーン管理

  Gun.cs

  銃撃処理、ターゲットのヒット判定

  Grab.cs

  VRでオブジェクトを掴む・投げる機能

  GrabO.cs

  物理演算を考慮した遠隔グラブ機能

  TeleportStraight.cs

  直線型テレポート処理

  TeleportCurve.cs

  カーブ型テレポート処理（曲線移動）

  TeleportCurved.cs

  高度なカーブテレポート処理

  PlayerMove.cs

  プレイヤーの移動・ジャンプ機能

  Tower.cs

  タワーのHP管理、攻撃処理

4. インストールと実行方法

  Unityのプロジェクトとして VRDefense をクローンします。

  Oculus Integration をインストールし、VR環境をセットアップします。

  Scenes/MainScene.unity を開き、プレイを開始します。

5. 今後の改善・アップデート予定

  敵の種類の増加: より多様な動きをする敵AIを追加予定。

  武器システムの拡張: 新しい武器（弓、グレネードなど）の追加。

  オンラインマルチプレイ: プレイヤー同士で協力して戦えるモードの実装。

映像


https://github.com/user-attachments/assets/0dada2f0-635b-4f3f-8d48-e880943ebe43




![image](https://github.com/user-attachments/assets/758dd0b7-4912-4067-a0f0-b98202e500d0)

![image](https://github.com/user-attachments/assets/d02f89b8-7e20-402b-9abb-6d6cffad7cbf)
