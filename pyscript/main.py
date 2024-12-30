import os
import json

def decode_mdplot(data: str) -> list[str]:
    in_tooken: bool = False
    tookens: list[str] = []
    current_tooken: str = ""
    for i in range(len(data)):
        if data[i] == "`":
            if in_tooken:
                tookens.append(current_tooken)
            in_tooken = not in_tooken
            current_tooken = ""
            continue
        current_tooken += data[i]
    return tookens

if __name__ == '__main__':
    if not os.path.exists(".\\plot"):
        os.chdir(os.path.dirname(os.getcwd()))
    current_directory = os.getcwd()
    print(current_directory)
    # 检查文件夹的各个文件
    plot_dir: str = current_directory+"\\plot\\"
    plot_json_dir: str = current_directory+"\\plotJson\\"
    files: list[str] = os.listdir(plot_dir)
    print(files)
    os.makedirs(plot_json_dir, exist_ok=True)

    for file in files:
        this_plot_dir: str = plot_json_dir+file[:-3]+"\\"
        os.makedirs(this_plot_dir, exist_ok=True)
        with open(plot_dir+file, "r", encoding='utf-8') as f:
            data: str = f.read()
            tookens: list[str] = decode_mdplot(data)
            i: int = 0
            while i < len(tookens):
                if tookens[i] != "file":
                    break
                with open(this_plot_dir+tookens[i+1], "w", encoding='utf-8') as f:
                    i += 2
                    json_file_data = {}
                    json_line = {}
                    while i < len(tookens) and tookens[i] != "file":
                        caption_index = tookens[i]
                        actorName = tookens[i+1]
                        caption = tookens[i+2]
                        if tookens[i+3] == "caption":
                            json_line = {
                                caption_index: {
                                    "actorName": actorName,
                                    "caption": caption,
                                    "next": {
                                        "caption": int(tookens[i+4]),
                                    }
                                }
                            }
                            i += 5
                        elif tookens[i+3] == "exit":
                            json_line = {
                                caption_index: {
                                    "actorName": actorName,
                                    "caption": caption,
                                    "next": {
                                        "exit": int(tookens[i+4])
                                    }
                                }
                            }
                            i += 5
                        elif tookens[i+3] == "choose":
                            i = i + 4
                            texts = []
                            # 读取选项
                            while True:
                                texts.append(tookens[i])
                                texts.append(tookens[i+1])
                                texts.append(tookens[i+2])
                                i += 3
                                if tookens[i] == "endChoose":
                                    i += 1
                                    break
                            texts_json = {}
                            print("texts", texts)
                            for j in range(len(texts) // 3):
                                print("texts_json", texts_json)
                                texts_json.update({
                                    j: {
                                        "text": texts[j*3],
                                        "next": {
                                            texts[j*3+1]: int(texts[j*3+2])
                                        }
                                    }
                                })
                            a_json = {
                                    "actorName": actorName,
                                    "caption": caption
                                }
                            a_json.update(texts_json)
                            json_line = {
                                caption_index: a_json
                            }
                            print("before", json_line)
                            print("texts_json", texts_json)
                            print("after", json_line)
                        else:
                            break
                        json_file_data.update(json_line)
                    print(json_file_data)
                    json.dump(json_file_data, f, ensure_ascii=False, indent=4)
