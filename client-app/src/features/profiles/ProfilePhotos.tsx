import { observer } from "mobx-react-lite";
import { Card, Header, Tab, Image, Grid, Button } from "semantic-ui-react";
import { Photo, Profile } from "../../app/models/profile";
import { useStore } from "../../app/strores/store";
import { SyntheticEvent, useState } from "react";
import PhotoUploadWidget from "../../app/common/imageUpload/PhotoUploadWidget";

interface Props {
    profile: Profile
}

export default observer(function ProfilePhotos({profile}:Props) {
    
    const { profileStore: { isCurrentUser, uploadPhoto, uploading, loading, setMainPhoto, deletePhoto } } = useStore();
    const [addPhotoMode, setAddPhotoMode] = useState(false);
    const [target, setTarget] = useState("");

    function handleSetMainPhoto(photo: Photo, e: SyntheticEvent<HTMLButtonElement>) {
        setTarget(e.currentTarget.name);
        setMainPhoto(photo);
    }
    function handleDeletePhoto(photo: Photo, e: SyntheticEvent<HTMLButtonElement>) {
        setTarget(e.currentTarget.name);
        deletePhoto(photo);
    }

    function handlePhotoUpload(file: Blob) {
        uploadPhoto(file).then(() => setAddPhotoMode(false));
    }

    return (
        <Tab.Pane>
            <Grid>
                <Grid.Column width={16}>
                    <Header floated="left" icon='image' content="Photos" />
                    {isCurrentUser && (
                        <Button
                            onClick={() => setAddPhotoMode(!addPhotoMode)}
                            floated="right"
                            basic content={addPhotoMode ? 'Cancel' : "Add Photo"} />
                    )}
                </Grid.Column>
                <Grid.Column width={16}>
                    {addPhotoMode ? (
                        <PhotoUploadWidget uploadPhoto={handlePhotoUpload} loading={uploading} />
                    ) : (<Card.Group itemsPerRow={5}>
                        {profile.photos?.map((photo) => (
                            <Card key={photo.id}>
                                <Image src={photo.url} />
                                {isCurrentUser && (
                                    <Button.Group fluid widths={2}>
                                        <Button
                                            color="green"
                                            content="Main"
                                            name={"main" + photo.id}
                                            disabled={photo.isMain}
                                            loading={target === 'main' + photo.id && loading}
                                            onClick={e => handleSetMainPhoto(photo, e)}
                                            basic
                                        />
                                        <Button
                                            loading={target === photo.id && loading}
                                            onClick={e => handleDeletePhoto(photo, e)}
                                            disabled={photo.isMain}
                                            name={photo.id}
                                            color="red"
                                            icon="trash"
                                            basic
                                            />
                                    </Button.Group>
                                )}
                            </Card>
                        ))}
                        
                    </Card.Group>)}
                </Grid.Column>
            </Grid>
        </Tab.Pane>
    );
});